using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotations.Core.Entities;

public class DatasetEntry
{
    public required int ImageSeriesId { get; set; }

    public required int ImageId { get; set; }
        
    public int OrderNumber { get; set; }
}
