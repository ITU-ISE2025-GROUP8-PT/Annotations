namespace Annotations.Core.VesselObjects;


    public class VesselPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Text { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class VesselSegment
    {
        public VesselPoint? StartPoint { get; set; }
        public VesselPoint? EndPoint { get; set; }
        public string Text { get; set; }
        public double Thickness { get; set; }
        public string Type { get; set; }
    }


    public class VesselAnnotation
    {
        public List<VesselPoint?> Points { get; set; }
        public List<VesselSegment> Segments { get; set; }
    }
