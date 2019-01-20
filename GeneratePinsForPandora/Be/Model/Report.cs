namespace GeneratePinsForPandora.Be.Model
{
    public class Report
    {
        public string Name { get; set; }
        public string Area { get; set; }
        public string Type { get; set; }
        public int ObjectsCount { get; set; }
        public int OneObjectPeopleCount { get; set; }
        public int AvgPrice { get; set; }
        public int PopulationDensity { get; set; }
        public int Population { get; set; }
        
        public int[] GrafA { get; set; }
        public int[] GrafB1 { get; set; }
        public int[] GrafB2 { get; set; }
        public double[] GrafC { get; set; }
    }
}