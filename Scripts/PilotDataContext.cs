namespace XWingBuilder
{
    public class PilotDataContext
    {
        public string PilotName { get; set; }
        public string Image { get; set; }


        public Pilot source;
        public PilotDataContext(Pilot _source)
        {
            PilotName = _source.Name;
            Image = _source.ImageSource;

            source = _source;
        }
    }
}
