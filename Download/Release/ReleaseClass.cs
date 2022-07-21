namespace ARX_PME_Updater
{
    /// <summary>
    /// Describe release.json.
    /// </summary>
    internal class ReleaseClass
    {
        public class Assets
        {
            public List<Info> updated { get; set; }
            public List<Info> removed { get; set; }
            public List<Info> added { get; set; }
            public List<Info> unchanged { get; set; }
        }

        public class Root
        {
            public string name { get; set; }
            public string description { get; set; }
            public string version { get; set; }
            public string previousVersion { get; set; }
            public Assets assets { get; set; }
        }

        public class Info
        {
            public string name { get; set; }
            public string baseFolder { get; set; }
            public string path { get; set; }
        }

    }
}
