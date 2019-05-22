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

using OpenNI;


/// @brief a built in query with primesense vendor limitation
/// 
/// This is an example query which limits to primesense as a specific vendor.
/// @ingroup OpenNIBasicObjects
public class NIPrimeSenseQuery : NIQuery
{
    /// constructor
    public NIPrimeSenseQuery()
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_query = new Query();
        m_query.SetVendor("PrimeSense");        
    }
}
