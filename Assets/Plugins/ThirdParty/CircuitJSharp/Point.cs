/*    
    Copyright (C) Paul Falstad and Iain Sharp
    
    This file is part of CircuitJS1.

    CircuitJS1 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    CircuitJS1 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CircuitJS1.  If not, see <http://www.gnu.org/licenses/>.
*/

public class Point
{
    public int x;

    public Point(int i)
    {
        x = i;
    }

    public Point(Point p)
    {
        x = p.x;
    }

    public Point()
    {
        x = 0;
    }

    public void setLocation(Point p)
    {
        x = p.x;
    }

    public override string ToString()
    {
        return "Point(" + x + ")";
    }

    public override bool Equals(object other)
    {
        bool result = false;
        if (other is Point)
        {
            Point that = (Point)other;
            result = (this.x == that.x);
        }

        return result;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode();
    }
}