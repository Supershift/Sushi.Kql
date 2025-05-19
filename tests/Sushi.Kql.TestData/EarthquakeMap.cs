using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.TestData;
public class EarthquakeMap : DataMap<Earthquake>
{
    public EarthquakeMap()
    {
        Map(x => x.TimeStamp);
        Map(x => x.Magnitude);
        Map(x => x.DepthInMeters);
        Map(x => x.Location.Latitude);
        Map(x => x.Location.Longitude);        
    }

    public override string TableName => "Earthquakes";
}
