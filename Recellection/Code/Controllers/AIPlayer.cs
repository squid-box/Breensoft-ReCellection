using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Views;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    class AIPlayer
    {

        /**
         * Variables
         */
        private AIView View;
        private Coordinate[] InterrestPoints;
        private Coordinate[] EnemyPoints;
        private int Threshold;




        /**
         * Methods
         */
        public void MakeMove(){

            for (int i = 0; i < InterrestPoints.Length; i++)
            {
                Coordinate Current = InterrestPoints[i];

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
                    Coordinate Current = EnemyPoints[i];
                    Coordinate Nearby = getClosestInterrestPoint(Current);
                    if (distanceBetween(Current, Nearby) > Threshold)
                    {
                        Nearby = calculatePointNear(Current);
                        InterrestPoints[InterrestPoints.Length] = Nearby;
                    }
                    sendUnits(Nearby);
                }
            }
        }

        private bool containsResourcePoint(Coordinate Current)
        {
            throw new NotImplementedException();
        }

        private void calculateWeight(Coordinate Current)
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

        private Coordinate getClosestInterrestPoint(Coordinate Current)
        {
            throw new NotImplementedException();
        }

        private int distanceBetween(Coordinate Current, Coordinate Nearby)
        {
            throw new NotImplementedException();
        }

        private Coordinate calculatePointNear(Coordinate Current)
        {
            throw new NotImplementedException();
        }

        private void evaluateResourcePoint(Coordinate Point)
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

        private bool harvesting(Coordinate Point)
        {
            throw new NotImplementedException();
        }

        private bool canHoldPoint(Coordinate Point)
        {
            throw new NotImplementedException();
        }

        private void issueBuildOrder(Coordinate Point, string p)
        {
            throw new NotImplementedException();
        }

        private void sendUnits(Coordinate Point)
        {
            throw new NotImplementedException();
        }
    }
}
