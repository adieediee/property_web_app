using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Data.ViewModels
{
    public class PropertyScreenViewModel
    {
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Názov nehnuteľnosti je povinný.")]
        [StringLength(100, ErrorMessage = "Názov nehnuteľnosti môže mať maximálne 100 znakov.")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Mesto je povinné.")]
        [StringLength(50, ErrorMessage = "Mesto môže mať maximálne 50 znakov.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Štát je povinný.")]
        [StringLength(50, ErrorMessage = "Štát môže mať maximálne 50 znakov.")]
        public string State { get; set; }
        public string MainImage { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Cena musí byť v rozmedzí od 0 do 1 000 000.")]
        public decimal Price { get; set; }

        [Range(0, 20, ErrorMessage = "Počet spální musí byť v rozmedzí od 0 do 20.")]
        public short NumberOfBedrooms { get; set; }

        [Range(0, 20, ErrorMessage = "Počet kúpeľní musí byť v rozmedzí od 0 do 20.")]
        public short NumberOfBathrooms { get; set; }

        [Range(0, 10_000, ErrorMessage = "Rozloha musí byť v rozmedzí od 0 do 10 000 m².")]
        public int Area { get; set; }

        [StringLength(1000, ErrorMessage = "Popis môže mať maximálne 1000 znakov.")]
        public string Description { get; set; }
        public string TenantName { get; set; }
        public string TenantAvatar { get; set; } = "/img/default-avatar.jpg";
    }
}
