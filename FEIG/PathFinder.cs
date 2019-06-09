// Written By Ben Gordon

using FEIG.Map;
using FEIG.Units;
using Microsoft.Xna.Framework;
using System;

namespace FEIG
{
    public class PathFinder
    {
        public bool ValidPath(Unit unit, Point start, Point goal)
        {
            // Handle unreachable goals
            if (start == goal)
                return true;

            if (!unit.CanMoveTo(goal))
                return false;

            Point currentPoint = start;

            if (ValidAlongX(unit, start, goal))
            {
                currentPoint.X = goal.X;

                if (ValidAlongY(unit, currentPoint, goal))
                    return true;
                else
                    return false;
            }
            else
            {
                if (ValidAlongY(unit, start, goal))
                {
                    currentPoint.Y = goal.Y;

                    if (ValidAlongX(unit, currentPoint, goal))
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }

        private bool ValidAlongX(Unit unit, Point A, Point B)
        {
            if (A.X == B.X)
                return true;

            Point dif = B - A;
            Point current = A;
            Point nextPoint = current;
            int distanceMod = 0;

            nextPoint.X += Math.Sign(dif.X);
            TileType tile = Level.GetTile(nextPoint).type;

            if (!unit.CanPass(tile))
                return false;

            // Can't pass onto next point if the current point is a forest and we are infantry (infantry get slowed down by forests.)
            if (unit.moveType == MoveType.Infantry && tile == TileType.Forest)
                distanceMod--;

            if (unit.DistanceTo(B) > Unit.moveRanges[(int)unit.moveType] + distanceMod)
                return false;

            // Cannot pass through enemies
            Unit nextPointUnit = Game1.GetUnit(nextPoint);
            if (nextPointUnit != null && nextPointUnit.team != unit.team)
                return false;

            current = nextPoint;

            return ValidAlongX(unit, current, B);
        }

        private bool ValidAlongY(Unit unit, Point A, Point B)
        {
            if (A.Y == B.Y)
                return true;

            Point dif = B - A;
            Point current = A;
            Point nextPoint = current;
            int distanceMod = 0;

            nextPoint.Y += Math.Sign(dif.Y);
            TileType tile = Level.GetTile(nextPoint).type;

            if (!unit.CanPass(tile))
                return false;

            if (unit.moveType == MoveType.Infantry && tile == TileType.Forest)
                distanceMod -= 1;

            // Can't pass onto next point if the current point is a forest and we are infantry (infantry get slowed down by forests.)
            if (unit.DistanceTo(B) > Unit.moveRanges[(int)unit.moveType] + distanceMod)
                return false;

            // Cannot pass through enemies
            Unit nextPointUnit = Game1.GetUnit(nextPoint);
            if (nextPointUnit != null && nextPointUnit.team != unit.team)
                return false;

            current = nextPoint;

            return ValidAlongY(unit, current, B);
        }
    }
}
