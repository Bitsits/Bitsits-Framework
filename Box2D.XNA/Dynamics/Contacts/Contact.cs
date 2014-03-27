/*
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Box2D.XNA
{
    /// <summary>
    /// A contact edge is used to connect bodies and contacts together
    /// in a contact graph where each body is a node and each contact
    /// is an edge. A contact edge belongs to a doubly linked list
    /// maintained in each attached body. Each contact has two contact
    /// nodes, one for each attached body.
    /// </summary>
    public class ContactEdge
    {
        /// <summary>
        /// provides quick access to the other body attached.
        /// </summary>
        public Body Other;

        /// <summary>
        /// the contact
        /// </summary>
        public Contact Contact;

        /// <summary>
        /// the previous contact edge in the body's contact list
        /// </summary>
        public ContactEdge Prev;

        /// <summary>
        /// the next contact edge in the body's contact list
        /// </summary>
        public ContactEdge Next;
    };

    [Flags]
    public enum ContactFlags
    {
        None = 0,

        /// <summary>
        /// Used when crawling contact graph when forming islands.
        /// </summary>
        Island = 0x0001,

        /// <summary>
        /// Set when the shapes are touching.
        /// </summary>
        Touching = 0x0002,

        /// <summary>
        /// This contact can be disabled (by user)
        /// </summary>
        Enabled = 0x0004,

        /// <summary>
        /// This contact needs filtering because a fixture filter was changed.
        /// </summary>
        Filter = 0x0008,
    };

    /// <summary>
    /// The class manages contact between two shapes. A contact exists for each overlapping
    /// AABB in the broad-phase (except if filtered). Therefore a contact object may exist
    /// that has no contact points.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Get the contact manifold. Do not modify the manifold unless you understand the
        /// internals of Box2D.
        /// </summary>
        /// <param name="manifold"></param>
        public void GetManifold(out Manifold manifold)
        {
            manifold = _manifold;
        }

        /// <summary>
        /// Get the world manifold.
        /// </summary>
        /// <param name="worldManifold"></param>
        public void GetWorldManifold(out WorldManifold worldManifold)
        {
            Body bodyA = _fixtureA.GetBody();
            Body bodyB = _fixtureB.GetBody();
            Shape shapeA = _fixtureA.GetShape();
            Shape shapeB = _fixtureB.GetShape();

            Transform xfA, xfB;
            bodyA.GetTransform(out xfA);
            bodyB.GetTransform(out xfB);

            worldManifold = new WorldManifold(ref _manifold, ref xfA, shapeA._radius, ref xfB, shapeB._radius);
        }

        /// <summary>
        /// Is this contact touching?
        /// </summary>
        /// <returns></returns>
        public bool IsTouching()
        {
            return (_flags & ContactFlags.Touching) == ContactFlags.Touching;
        }

        /// <summary>
        /// Enable/disable this contact. This can be used inside the pre-solve
        /// contact listener. The contact is only disabled for the current
        /// time step (or sub-step in continuous collisions).
        /// </summary>
        /// <param name="flag"></param>
        public void SetEnabled(bool flag)
        {
            if (flag)
            {
                _flags |= ContactFlags.Enabled;
            }
            else
            {
                _flags &= ~ContactFlags.Enabled;
            }
        }

        /// <summary>
        /// Has this contact been disabled?
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return (_flags & ContactFlags.Enabled) == ContactFlags.Enabled;
        }

        /// <summary>
        /// Get the next contact in the world's contact list.
        /// </summary>
        /// <returns></returns>
        public Contact GetNext()
        {
            return _next;
        }

        /// <summary>
        /// Get the first fixture in this contact.
        /// </summary>
        /// <returns></returns>
        public Fixture GetFixtureA()
        {
            return _fixtureA;
        }

        /// <summary>
        /// Get the second fixture in this contact.
        /// </summary>
        /// <returns></returns>
        public Fixture GetFixtureB()
        {
            return _fixtureB;
        }

        /// <summary>
        /// Flag this contact for filtering. Filtering will occur the next time step.
        /// </summary>
        public void FlagForFiltering()
        {
            _flags |= ContactFlags.Filter;
        }

        internal Contact(Fixture fA, Fixture fB)
        {
            Reset(fA, fB);
        }

        internal void Reset(Fixture fA, Fixture fB)
        {
            _flags = ContactFlags.Enabled;

            _fixtureA = fA;
            _fixtureB = fB;

            _manifold._pointCount = 0;

            _prev = null;
            _next = null;

            _nodeA.Contact = null;
            _nodeA.Prev = null;
            _nodeA.Next = null;
            _nodeA.Other = null;

            _nodeB.Contact = null;
            _nodeB.Prev = null;
            _nodeB.Next = null;
            _nodeB.Other = null;

            _toiCount = 0;
        }

        /// <summary>
        /// Update the contact manifold and touching status.
        /// Note: do not assume the fixture AABBs are overlapping or are valid.
        /// </summary>
        /// <param name="listener"></param>
        internal void Update(IContactListener listener)
        {
            Manifold oldManifold = _manifold;

            // Re-enable this contact.
            _flags |= ContactFlags.Enabled;

            bool touching = false;
            bool wasTouching = (_flags & ContactFlags.Touching) == ContactFlags.Touching;

            bool sensorA = _fixtureA.IsSensor();
            bool sensorB = _fixtureB.IsSensor();
            bool sensor = sensorA || sensorB;

            Body bodyA = _fixtureA.GetBody();
            Body bodyB = _fixtureB.GetBody();
            Transform xfA; bodyA.GetTransform(out xfA);
            Transform xfB; bodyB.GetTransform(out xfB);

            // Is this contact a sensor?
            if (sensor)
            {
                Shape shapeA = _fixtureA.GetShape();
                Shape shapeB = _fixtureB.GetShape();
                touching = AABB.TestOverlap(shapeA, shapeB, ref xfA, ref xfB);

                // Sensors don't generate manifolds.
                _manifold._pointCount = 0;
            }
            else
            {
                Evaluate(ref _manifold, ref xfA, ref xfB);
                touching = _manifold._pointCount > 0;

                // Match old contact ids to new contact ids and copy the
                // stored impulses to warm start the solver.
                for (int i = 0; i < _manifold._pointCount; ++i)
                {
                    ManifoldPoint mp2 = _manifold._points[i];
                    mp2.NormalImpulse = 0.0f;
                    mp2.TangentImpulse = 0.0f;
                    ContactID id2 = mp2.Id;

                    for (int j = 0; j < oldManifold._pointCount; ++j)
                    {
                        ManifoldPoint mp1 = oldManifold._points[j];

                        if (mp1.Id.Key == id2.Key)
                        {
                            mp2.NormalImpulse = mp1.NormalImpulse;
                            mp2.TangentImpulse = mp1.TangentImpulse;
                            break;
                        }
                    }
                    _manifold._points[i] = mp2;
                }

                if (touching != wasTouching)
                {
                    bodyA.SetAwake(true);
                    bodyB.SetAwake(true);
                }
            }

            if (touching)
            {
                _flags |= ContactFlags.Touching;
            }
            else
            {
                _flags &= ~ContactFlags.Touching;
            }

            if (wasTouching == false && touching == true)
            {
                listener.BeginContact(this);
            }

            if (wasTouching == true && touching == false)
            {
                listener.EndContact(this);
            }

            if (sensor == false)
            {
                listener.PreSolve(this, ref oldManifold);
            }
        }

        /// <summary>
        /// Evaluate this contact with your own manifold and transforms.
        /// </summary>
        /// <param name="manifold"></param>
        /// <param name="xfA"></param>
        /// <param name="xfB"></param>
        internal void Evaluate(ref Manifold manifold, ref Transform xfA, ref Transform xfB)
        {
            switch (_type)
            {
                case ContactType.Polygon:
                    Collision.CollidePolygons(ref manifold,
                        (PolygonShape)_fixtureA.GetShape(), ref xfA,
                        (PolygonShape)_fixtureB.GetShape(), ref xfB);
                    break;
                case ContactType.PolygonAndCircle:
                    Collision.CollidePolygonAndCircle(ref manifold,
                            (PolygonShape)_fixtureA.GetShape(), ref xfA,
                            (CircleShape)_fixtureB.GetShape(), ref xfB);
                    break;
                case ContactType.Circle:
                    Collision.CollideCircles(ref manifold,
                            (CircleShape)_fixtureA.GetShape(), ref xfA,
                            (CircleShape)_fixtureB.GetShape(), ref xfB);
                    break;
            }
        }

        internal static ContactType[,] s_registers = new ContactType[,] 
        {
            { 
              ContactType.Circle,
              ContactType.PolygonAndCircle
            },
            { 
              ContactType.PolygonAndCircle, 
              ContactType.Polygon
            },
        };

        internal static Contact Create(Fixture fixtureA, Fixture fixtureB)
        {
            ShapeType type1 = fixtureA.ShapeType;
            ShapeType type2 = fixtureB.ShapeType;

            Debug.Assert(ShapeType.Unknown < type1 && type1 < ShapeType.TypeCount);
            Debug.Assert(ShapeType.Unknown < type2 && type2 < ShapeType.TypeCount);

            Contact c;
            var pool = fixtureA._body._world._contactPool;
            if (pool.Count > 0)
            {
                c = pool.Dequeue();
                if (type1 >= type2)
                {
                    c.Reset(fixtureA, fixtureB);
                }
                else
                {
                    c.Reset(fixtureB, fixtureA);
                }
            }
            else
            {
                if (type1 >= type2)
                {
                    c = new Contact(fixtureA, fixtureB);
                }
                else
                {
                    c = new Contact(fixtureB, fixtureA);
                }
            }

            c._type = Contact.s_registers[(int)type1, (int)type2];

            return c;
        }

        internal void Destroy()
        {
            _fixtureA._body._world._contactPool.Enqueue(this);
            Reset(null, null);
        }

        internal enum ContactType
        {
            Polygon,
            PolygonAndCircle,
            Circle,
        }

        private ContactType _type;
        internal ContactFlags _flags;

        /// <summary>
        /// World pool and list pointers.
        /// </summary>
        internal Contact _prev, _next;

        /// <summary>
        /// Nodes for connecting bodies.
        /// </summary>
        internal ContactEdge _nodeA = new ContactEdge(), _nodeB = new ContactEdge();

        internal Fixture _fixtureA;
        internal Fixture _fixtureB;

        internal Manifold _manifold;

        internal int _toiCount;
    };
}
