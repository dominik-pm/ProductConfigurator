using Model.Interfaces;

namespace Model.Wrapper {
    public class SavedConfigWrapper : IConfigId {
        public string ConfigId { get; set; }
        public string SavedName { get; set; }
    }
}