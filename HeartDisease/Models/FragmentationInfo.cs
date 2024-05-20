<<<<<<< HEAD
﻿namespace HeartDisease.Models {
    public class FragmentationInfo {
        public string DatabaseName { get; set; }
=======
﻿namespace HeartDisease.Models
{
    public class IndexFragmentationHistory
    {
        public int ID { get; set; }
>>>>>>> master
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public float FragmentationPercent { get; set; }
        public DateTime CheckDate { get; set; }

    }
}
