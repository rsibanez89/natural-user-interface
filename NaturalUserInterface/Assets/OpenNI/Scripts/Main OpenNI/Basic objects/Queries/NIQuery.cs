/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
using UnityEngine;
using OpenNI;


/// @brief abstraction of queries
/// 
/// This class is responsible for abstracting a query limiting the nodes created
/// @note it is mono-behavior so we can drag & drop it.
/// @ingroup OpenNIBasicObjects
public class NIQuery : MonoBehaviour
{
    /// The query to use. For the default (null) query this is null.
    protected Query m_query;
    /// Accessor - The query to use. For the default (null) query this is null.
    public Query UsedQuery
    {
        get { return m_query; }
    }

    /// constructor
    public NIQuery()
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_query = null;
    }
}
