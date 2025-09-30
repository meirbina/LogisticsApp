using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class ManifestListVM
    {
        public IEnumerable<Manifest> Manifests { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}