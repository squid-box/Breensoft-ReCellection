using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    class AIPlayer
    {

        /**
         * Variables
         */
        private AIView View;
        private Vector2[] InterrestPoints;
        private Vector2[] EnemyPoints;
        private int Threshold;




        /**
         * Methods
         */
        public void MakeMove(){

            for (int i = 0; i < InterrestPoints.Length; i++)
            {
                Vector2 Current = InterrestPoints[i];

                if (containsResourcePoint(Current))
                {
                    evaluateResourcePoint(Current);
                }
                else
                {
                    calculateWeight(Current);
                }
            }

            if (EnemyPoints.Length == 0)
            {
                explore(calculateScoutDirection());
            }
            else
            {
                for (int i = 0; i < EnemyPoints.Length; i++)
                {
                    Vector2 Current = EnemyPoints[i];
                    Vector2 Nearby = getClosestInterrestPoint(Current);
                    if (distanceBetween(Current, Nearby) > Threshold)
                    {
                        Nearby = calculatePointNear(Current);
                        InterrestPoints[InterrestPoints.Length] = Nearby;
                    }
                    sendUnits(Nearby);
                }
            }
        }

        private bool containsResourcePoint(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private void calculateWeight(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private void explore(object p)
        {
            throw new NotImplementedException();
        }

        private object calculateScoutDirection()
        {
            throw new NotImplementedException();
        }

        private Vector2 getClosestInterrestPoint(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private int distanceBetween(Vector2 Current, Vector2 Nearby)
        {
            throw new NotImplementedException();
        }

        private Vector2 calculatePointNear(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private void evaluateResourcePoint(Vector2 Point)
        {
            //Resource point but we already have it.
            if (harvesting(Point))
                return;

            if (canHoldPoint(Point))
            {
                issueBuildOrder(Point, "Resource");
            }
            else
            {
                sendUnits(Point);
            }
        }

        private bool harvesting(Vector2 Point)
        {
            throw new NotImplementedException();
        }

        private bool canHoldPoint(Vector2 Point)
        {
            throw new NotImplementedException();
        }

        private void issueBuildOrder(Vector2 Point, string p)
        {
            throw new NotImplementedException();
        }

        private void sendUnits(Vector2 Point)
        {
            throw new NotImplementedException();
        }
    }
}
