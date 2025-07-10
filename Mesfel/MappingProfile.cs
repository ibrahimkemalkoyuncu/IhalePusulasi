using AutoMapper;
using Mesfel.Models;
using Mesfel.ViewModel;

namespace Mesfel
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ihale, IhaleViewModel>()
                            .ForMember(dest => dest.ToplamTeklifSayisi,
                                       opt => opt.MapFrom(src => src.IhaleTeklifleri.Count))
                            .ForMember(dest => dest.OrtalamaTeklif,
                                       opt => opt.MapFrom(src => src.IhaleTeklifleri.Average(t => t.TeklifTutari)))
                            .ForMember(dest => dest.Kategoriler,
                                       opt => opt.MapFrom(src => src.IhaleKategorileri.Select(k => k.Kategori.KategoriAdi)));

            CreateMap<IhaleKalemi, IhaleKalemViewModel>();
            CreateMap<IhaleTeklif, IhaleTeklifViewModel>();
        }
    }



    
}
