using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotations.Core.Entities;

public class ImageSeriesEntry
{
    public long ImageSeriesId { get; set; }
    public required string ImageId { get; set; }
    public int OrderNumber { get; set; }
}
