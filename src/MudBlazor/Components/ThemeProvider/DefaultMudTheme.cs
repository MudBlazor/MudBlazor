namespace MudBlazor
{
    public class DefaultMudTheme
    {
        //Core Layout Surfaces
        public string Color_Background { get; set; } = "#fff";
        public string Color_Background_Grey { get; set; } = "#f5f5f5";

        public string Color_Surface { get; set; } = "#fff";
        public string Color_On_Surface { get; set; } = "#444";

        public string Color_AppBar { get; set; } = "#594ae2";
        public string Color_On_AppBar { get; set; } = "#fff";

        public string Color_Drawer { get; set; } = "#fff";
        public string Color_On_Drawer { get; set; } = "#444";


        //Text Only
        public string Color_Text_Default { get; set; } = "#6a6a6a";


        //Theme Main Colors
        public string Color_Default { get; set; } = "#000";
        public string Color_On_Default { get; set; } = "#000";

        public string Color_Primary { get; set; } = "#594ae2";
        public string Color_On_Primary { get; set; } = "#fff";

        public string Color_Secondary { get; set; } = "#FF458A";
        public string Color_On_Secondary { get; set; } = "#fff";


        //Theme Alert & Notification Colors
        public string Color_Info { get; set; } = "#2196f3";
        public string Color_Success { get; set; } = "#4caf50";
        public string Color_Warning { get; set; } = "#ff9800";
        public string Color_Danger { get; set; } = "#f44336";
        public string Color_Dark { get; set; } = "#272c34";
        public string Color_On_Alert { get; set; } = "#fff";


        //Theme Border Colors, Todo remove or make less of them?
        public string Color_Border_Default { get; set; } = "#e0e0e0";
        public string Color_Border_Outlines { get; set; } = "#c4c4c4";
        public string Color_Border_Inputs { get; set; } = "#878787";


        //Theme Default BorderRadius
        public string BorderRadius_Default { get; set; } = "4px";
    }
}
