#region Header

//    XMC, a Minecraft SMP server.
//    Copyright (C) 2011 XMC. All rights reserved.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion Header

using System;
using System.Collections.Generic;

public class Builder<T>
{
    #region Fields

    List<T> _contents = new List<T>();

    #endregion Fields

    #region Methods

    public void Append(T param)
    {
        _contents.Add(param);
    }

    public void Append(T[] param)
    {
        foreach (var item in param)
        {
            _contents.Add(item);
        }
    }

    public T[] ToArray()
    {
        T[] returnValue = new T[_contents.Count];
        for (int i = 0; i < _contents.Count; i++)
        {
            returnValue[i] = _contents[i];
        }
        return returnValue;
    }

    #endregion Methods
}