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


// info about each row/column of the matrix for simplification purposes
public class RowInfo
{
    public static int ROW_NORMAL = 0; // ordinary value
    public static int ROW_CONST = 1; // value is constant
    public int type, mapCol, mapRow;
    public double value;
    public bool rsChanges; // row's right side changes
    public bool lsChanges; // row's left side changes
    public bool dropRow; // row is not needed in matrix

    public RowInfo()
    {
        type = ROW_NORMAL;
    }
}