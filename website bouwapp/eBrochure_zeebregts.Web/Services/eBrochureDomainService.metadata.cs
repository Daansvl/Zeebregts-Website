
namespace eBrochure_zeebregts.Web
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Data.Objects.DataClasses;
	using System.Linq;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;


	// The MetadataTypeAttribute identifies BouwnummersMetadata as the class
	// that carries additional metadata for the Bouwnummers class.
	[MetadataTypeAttribute(typeof(Bouwnummers.BouwnummersMetadata))]
	public partial class Bouwnummers
	{

		// This class allows you to attach custom attributes to properties
		// of the Bouwnummers class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class BouwnummersMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private BouwnummersMetadata()
			{
			}

			public string B_ID { get; set; }

			public bouwTypes bouwTypes { get; set; }

			public string F_NR { get; set; }

			public Fase Fase { get; set; }

			public string Omschrijving { get; set; }

			public string PR_NR { get; set; }

			public PRoject PRoject { get; set; }

			public string T_NR { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies bouwTypesMetadata as the class
	// that carries additional metadata for the bouwTypes class.
	[MetadataTypeAttribute(typeof(bouwTypes.bouwTypesMetadata))]
	public partial class bouwTypes
	{

		// This class allows you to attach custom attributes to properties
		// of the bouwTypes class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class bouwTypesMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private bouwTypesMetadata()
			{
			}

			public EntityCollection<Bouwnummers> Bouwnummers { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<RuimtesPerType> RuimtesPerType { get; set; }

			public string Soort { get; set; }

			public string T_ID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies Bron_RuimtesMetadata as the class
	// that carries additional metadata for the Bron_Ruimtes class.
	[MetadataTypeAttribute(typeof(Bron_Ruimtes.Bron_RuimtesMetadata))]
	public partial class Bron_Ruimtes
	{

		// This class allows you to attach custom attributes to properties
		// of the Bron_Ruimtes class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class Bron_RuimtesMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private Bron_RuimtesMetadata()
			{
			}

			public string BR_ID { get; set; }

			public PakketGroep PakketGroep { get; set; }

			public string PG_NR { get; set; }

			public Nullable<int> PLAFONDHOOGTE { get; set; }

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }

			public string tekeningPath { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies CategorieënMetadata as the class
	// that carries additional metadata for the Categorieën class.
	[MetadataTypeAttribute(typeof(Categorieën.CategorieënMetadata))]
	public partial class Categorieën
	{

		// This class allows you to attach custom attributes to properties
		// of the Categorieën class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class CategorieënMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private CategorieënMetadata()
			{
			}

			public string C_ID { get; set; }

			public string HC_NR { get; set; }

			public HoofdCategorieën HoofdCategorieën { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<SubCats> SubCats { get; set; }

			public Nullable<int> Volgorde { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies FaseMetadata as the class
	// that carries additional metadata for the Fase class.
	[MetadataTypeAttribute(typeof(Fase.FaseMetadata))]
	public partial class Fase
	{

		// This class allows you to attach custom attributes to properties
		// of the Fase class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class FaseMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private FaseMetadata()
			{
			}

			public Nullable<decimal> AfrondingBoven { get; set; }

			public EntityCollection<Bouwnummers> Bouwnummers { get; set; }

			public Nullable<int> BTW { get; set; }

			public string F_ID { get; set; }

			public Nullable<decimal> Indexering { get; set; }

			public string Omschrijving { get; set; }

			public Nullable<decimal> Opslag { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies KleurenMetadata as the class
	// that carries additional metadata for the Kleuren class.
	[MetadataTypeAttribute(typeof(Kleuren.KleurenMetadata))]
	public partial class Kleuren
	{

		// This class allows you to attach custom attributes to properties
		// of the Kleuren class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class KleurenMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private KleurenMetadata()
			{
			}

			public string K_ID { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<Producten> Producten { get; set; }

			public Nullable<int> volgorde { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies KleurenSetOpbouwMetadata as the class
	// that carries additional metadata for the KleurenSetOpbouw class.
	[MetadataTypeAttribute(typeof(KleurenSetOpbouw.KleurenSetOpbouwMetadata))]
	public partial class KleurenSetOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the KleurenSetOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class KleurenSetOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private KleurenSetOpbouwMetadata()
			{
			}

			public string KKSC_ID { get; set; }

			public KleurenSets KleurenSets { get; set; }

			public string LKC_NR { get; set; }

			public string PD_SET_NR { get; set; }

			public ProductSets ProductSets { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies KleurenSetsMetadata as the class
	// that carries additional metadata for the KleurenSets class.
	[MetadataTypeAttribute(typeof(KleurenSets.KleurenSetsMetadata))]
	public partial class KleurenSets
	{

		// This class allows you to attach custom attributes to properties
		// of the KleurenSets class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class KleurenSetsMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private KleurenSetsMetadata()
			{
			}

			public EntityCollection<KleurenSetOpbouw> KleurenSetOpbouw { get; set; }

			public string LKC_ID { get; set; }

			public string omschrijving { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies MeerPrijzenRuimteOptieMetadata as the class
	// that carries additional metadata for the MeerPrijzenRuimteOptie class.
	[MetadataTypeAttribute(typeof(MeerPrijzenRuimteOptie.MeerPrijzenRuimteOptieMetadata))]
	public partial class MeerPrijzenRuimteOptie
	{

		// This class allows you to attach custom attributes to properties
		// of the MeerPrijzenRuimteOptie class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class MeerPrijzenRuimteOptieMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private MeerPrijzenRuimteOptieMetadata()
			{
			}

			public Nullable<decimal> meerprijs { get; set; }

			public string MPRO_ID { get; set; }

			public PakketPrijsgroep PakketPrijsgroep { get; set; }

			public string PP_NR { get; set; }

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies NabewerkingCategoriënMetadata as the class
	// that carries additional metadata for the NabewerkingCategoriën class.
	[MetadataTypeAttribute(typeof(NabewerkingCategoriën.NabewerkingCategoriënMetadata))]
	public partial class NabewerkingCategoriën
	{

		// This class allows you to attach custom attributes to properties
		// of the NabewerkingCategoriën class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class NabewerkingCategoriënMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private NabewerkingCategoriënMetadata()
			{
			}

			public EntityCollection<Nabewerkingen> Nabewerkingen { get; set; }

			public string NC_ID { get; set; }

			public string Omschrijving { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies NabewerkingenMetadata as the class
	// that carries additional metadata for the Nabewerkingen class.
	[MetadataTypeAttribute(typeof(Nabewerkingen.NabewerkingenMetadata))]
	public partial class Nabewerkingen
	{

		// This class allows you to attach custom attributes to properties
		// of the Nabewerkingen class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class NabewerkingenMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private NabewerkingenMetadata()
			{
			}

			public string N_ID { get; set; }

			public NabewerkingCategoriën NabewerkingCategoriën { get; set; }

			public EntityCollection<NabewerkingSetOpbouw> NabewerkingSetOpbouw { get; set; }

			public string NC_NR { get; set; }

			public string omschrijving { get; set; }

			public Nullable<int> volgorde { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies NabewerkingSetCombinatiesMetadata as the class
	// that carries additional metadata for the NabewerkingSetCombinaties class.
	[MetadataTypeAttribute(typeof(NabewerkingSetCombinaties.NabewerkingSetCombinatiesMetadata))]
	public partial class NabewerkingSetCombinaties
	{

		// This class allows you to attach custom attributes to properties
		// of the NabewerkingSetCombinaties class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class NabewerkingSetCombinatiesMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private NabewerkingSetCombinatiesMetadata()
			{
			}

			public NabewerkingSets NabewerkingSets { get; set; }

			public string NS_NR { get; set; }

			public string SCBP_NR { get; set; }

			public string SSCP_ID { get; set; }

			public SubCatPerPakket SubCatPerPakket { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies NabewerkingSetOpbouwMetadata as the class
	// that carries additional metadata for the NabewerkingSetOpbouw class.
	[MetadataTypeAttribute(typeof(NabewerkingSetOpbouw.NabewerkingSetOpbouwMetadata))]
	public partial class NabewerkingSetOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the NabewerkingSetOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class NabewerkingSetOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private NabewerkingSetOpbouwMetadata()
			{
			}

			public string N_NR { get; set; }

			public Nabewerkingen Nabewerkingen { get; set; }

			public NabewerkingSets NabewerkingSets { get; set; }

			public string NPS_ID { get; set; }

			public string NS_NR { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies NabewerkingSetsMetadata as the class
	// that carries additional metadata for the NabewerkingSets class.
	[MetadataTypeAttribute(typeof(NabewerkingSets.NabewerkingSetsMetadata))]
	public partial class NabewerkingSets
	{

		// This class allows you to attach custom attributes to properties
		// of the NabewerkingSets class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class NabewerkingSetsMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private NabewerkingSetsMetadata()
			{
			}

			public Nullable<int> KeuzeMax { get; set; }

			public Nullable<int> KeuzeStandaard { get; set; }

			public EntityCollection<NabewerkingSetCombinaties> NabewerkingSetCombinaties { get; set; }

			public EntityCollection<NabewerkingSetOpbouw> NabewerkingSetOpbouw { get; set; }

			public string NS_ID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies OngeldigeCombinatieOpbouwMetadata as the class
	// that carries additional metadata for the OngeldigeCombinatieOpbouw class.
	[MetadataTypeAttribute(typeof(OngeldigeCombinatieOpbouw.OngeldigeCombinatieOpbouwMetadata))]
	public partial class OngeldigeCombinatieOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the OngeldigeCombinatieOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class OngeldigeCombinatieOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private OngeldigeCombinatieOpbouwMetadata()
			{
			}

			public string OC_NR { get; set; }

			public string OCS_ID { get; set; }

			public OngeldigeCombinaties OngeldigeCombinaties { get; set; }

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies OngeldigeCombinatiesMetadata as the class
	// that carries additional metadata for the OngeldigeCombinaties class.
	[MetadataTypeAttribute(typeof(OngeldigeCombinaties.OngeldigeCombinatiesMetadata))]
	public partial class OngeldigeCombinaties
	{

		// This class allows you to attach custom attributes to properties
		// of the OngeldigeCombinaties class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class OngeldigeCombinatiesMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private OngeldigeCombinatiesMetadata()
			{
			}

			public string OC_ID { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<OngeldigeCombinatieOpbouw> OngeldigeCombinatieOpbouw { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies OptieProductCombosMetadata as the class
	// that carries additional metadata for the OptieProductCombos class.
	[MetadataTypeAttribute(typeof(OptieProductCombos.OptieProductCombosMetadata))]
	public partial class OptieProductCombos
	{

		// This class allows you to attach custom attributes to properties
		// of the OptieProductCombos class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class OptieProductCombosMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private OptieProductCombosMetadata()
			{
			}

			public int ID { get; set; }

			public string PD_SET_NR { get; set; }

			public ProductSets ProductSets { get; set; }

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies OptieTypesMetadata as the class
	// that carries additional metadata for the OptieTypes class.
	[MetadataTypeAttribute(typeof(OptieTypes.OptieTypesMetadata))]
	public partial class OptieTypes
	{

		// This class allows you to attach custom attributes to properties
		// of the OptieTypes class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class OptieTypesMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private OptieTypesMetadata()
			{
			}

			public Nullable<int> OFase { get; set; }

			public string OT_ID { get; set; }

			public EntityCollection<RuimteDelen> RuimteDelen { get; set; }

			public string Type { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies PakketGroepMetadata as the class
	// that carries additional metadata for the PakketGroep class.
	[MetadataTypeAttribute(typeof(PakketGroep.PakketGroepMetadata))]
	public partial class PakketGroep
	{

		// This class allows you to attach custom attributes to properties
		// of the PakketGroep class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class PakketGroepMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private PakketGroepMetadata()
			{
			}

			public EntityCollection<Bron_Ruimtes> Bron_Ruimtes { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<PakketGroepOpbouw> PakketGroepOpbouw { get; set; }

			public string PG_ID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies PakketGroepOpbouwMetadata as the class
	// that carries additional metadata for the PakketGroepOpbouw class.
	[MetadataTypeAttribute(typeof(PakketGroepOpbouw.PakketGroepOpbouwMetadata))]
	public partial class PakketGroepOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the PakketGroepOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class PakketGroepOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private PakketGroepOpbouwMetadata()
			{
			}

			public string Omschrijving { get; set; }

			public string P_NR { get; set; }

			public PakketGroep PakketGroep { get; set; }

			public Pakketten Pakketten { get; set; }

			public string PG_NR { get; set; }

			public string PGO_ID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies PakketPrijsgroepMetadata as the class
	// that carries additional metadata for the PakketPrijsgroep class.
	[MetadataTypeAttribute(typeof(PakketPrijsgroep.PakketPrijsgroepMetadata))]
	public partial class PakketPrijsgroep
	{

		// This class allows you to attach custom attributes to properties
		// of the PakketPrijsgroep class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class PakketPrijsgroepMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private PakketPrijsgroepMetadata()
			{
			}

			public EntityCollection<MeerPrijzenRuimteOptie> MeerPrijzenRuimteOptie { get; set; }

			public string Omschrijving { get; set; }

			public EntityCollection<Pakketten> Pakketten { get; set; }

			public string PP_ID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies PakkettenMetadata as the class
	// that carries additional metadata for the Pakketten class.
	[MetadataTypeAttribute(typeof(Pakketten.PakkettenMetadata))]
	public partial class Pakketten
	{

		// This class allows you to attach custom attributes to properties
		// of the Pakketten class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class PakkettenMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private PakkettenMetadata()
			{
			}

			public string P_ID { get; set; }

			public EntityCollection<PakketGroepOpbouw> PakketGroepOpbouw { get; set; }

			public PakketPrijsgroep PakketPrijsgroep { get; set; }

			public string PP_NR { get; set; }

			public EntityCollection<SubCatPerPakket> SubCatPerPakket { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies ProductCategoriënMetadata as the class
	// that carries additional metadata for the ProductCategoriën class.
	[MetadataTypeAttribute(typeof(ProductCategoriën.ProductCategoriënMetadata))]
	public partial class ProductCategoriën
	{

		// This class allows you to attach custom attributes to properties
		// of the ProductCategoriën class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class ProductCategoriënMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private ProductCategoriënMetadata()
			{
			}

			public string Omschrijving { get; set; }

			public string PC_ID { get; set; }

			public EntityCollection<Producten> Producten { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies ProductenMetadata as the class
	// that carries additional metadata for the Producten class.
	[MetadataTypeAttribute(typeof(Producten.ProductenMetadata))]
	public partial class Producten
	{

		// This class allows you to attach custom attributes to properties
		// of the Producten class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class ProductenMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private ProductenMetadata()
			{
			}

			public Nullable<int> breedte { get; set; }

			public string ImagePath { get; set; }

			public Kleuren Kleuren { get; set; }

			public Nullable<int> lengte { get; set; }

			public string Omschrijving { get; set; }

			public string PC_NR { get; set; }

			public string PD_ID { get; set; }

			public string PKC_NR { get; set; }

			public ProductCategoriën ProductCategoriën { get; set; }

			public string productcode { get; set; }

			public EntityCollection<ProductSetOpbouw> ProductSetOpbouw { get; set; }

			public Nullable<decimal> verpakkingstoeslag { get; set; }

			public Nullable<int> volgorde { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies ProductSetOpbouwMetadata as the class
	// that carries additional metadata for the ProductSetOpbouw class.
	[MetadataTypeAttribute(typeof(ProductSetOpbouw.ProductSetOpbouwMetadata))]
	public partial class ProductSetOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the ProductSetOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class ProductSetOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private ProductSetOpbouwMetadata()
			{
			}

			public string PD_NR { get; set; }

			public string PD_SET_NR { get; set; }

			public string PPB_ID { get; set; }

			public Producten Producten { get; set; }

			public ProductSets ProductSets { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies ProductSetsMetadata as the class
	// that carries additional metadata for the ProductSets class.
	[MetadataTypeAttribute(typeof(ProductSets.ProductSetsMetadata))]
	public partial class ProductSets
	{

		// This class allows you to attach custom attributes to properties
		// of the ProductSets class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class ProductSetsMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private ProductSetsMetadata()
			{
			}

			public Nullable<int> KeuzeMax { get; set; }

			public Nullable<int> KeuzeStandaard { get; set; }

			public EntityCollection<KleurenSetOpbouw> KleurenSetOpbouw { get; set; }

			public EntityCollection<OptieProductCombos> OptieProductCombos { get; set; }

			public string PD_SET_ID { get; set; }

			public EntityCollection<ProductSetOpbouw> ProductSetOpbouw { get; set; }

			public EntityCollection<SubCatPerPakket> SubCatPerPakket { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies PRojectMetadata as the class
	// that carries additional metadata for the PRoject class.
	[MetadataTypeAttribute(typeof(PRoject.PRojectMetadata))]
	public partial class PRoject
	{

		// This class allows you to attach custom attributes to properties
		// of the PRoject class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class PRojectMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private PRojectMetadata()
			{
			}

			public EntityCollection<Bouwnummers> Bouwnummers { get; set; }

			public string Omschrijving { get; set; }

			public string PR_ID { get; set; }

			public Nullable<int> projectnummer { get; set; }
		}
	}

	
	
	// The MetadataTypeAttribute identifies RuimteOpbouwMetadata as the class
	// that carries additional metadata for the RuimteOpbouw class.
	[MetadataTypeAttribute(typeof(RuimteOpbouw.RuimteOpbouwMetadata))]
	public partial class RuimteOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the RuimteOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class RuimteOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private RuimteOpbouwMetadata()
			{
			}

			public string R_NR { get; set; }

			public string ROS_ID { get; set; }

			public string RS_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }

			public RuimteS RuimteS { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies RuimteSMetadata as the class
	// that carries additional metadata for the RuimteS class.
	[MetadataTypeAttribute(typeof(RuimteS.RuimteSMetadata))]
	public partial class RuimteS
	{

		// This class allows you to attach custom attributes to properties
		// of the RuimteS class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class RuimteSMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private RuimteSMetadata()
			{
			}

			public string Omschrijving { get; set; }

			public string RS_ID { get; set; }

			public EntityCollection<RuimteOpbouw> RuimteOpbouw { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies RuimtesPerTypeMetadata as the class
	// that carries additional metadata for the RuimtesPerType class.
	[MetadataTypeAttribute(typeof(RuimtesPerType.RuimtesPerTypeMetadata))]
	public partial class RuimtesPerType
	{

		// This class allows you to attach custom attributes to properties
		// of the RuimtesPerType class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class RuimtesPerTypeMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private RuimtesPerTypeMetadata()
			{
			}

			public bouwTypes bouwTypes { get; set; }

			public string R_NR { get; set; }

			public string RPT_ID { get; set; }

			public RuimteDelen RuimteDelen { get; set; }

			public string T_NR { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies StandaardOnafhandelijkeRuimteOpbouwMetadata as the class
	// that carries additional metadata for the StandaardOnafhandelijkeRuimteOpbouw class.
	[MetadataTypeAttribute(typeof(StandaardOnafhandelijkeRuimteOpbouw.StandaardOnafhandelijkeRuimteOpbouwMetadata))]
	public partial class StandaardOnafhandelijkeRuimteOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the StandaardOnafhandelijkeRuimteOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class StandaardOnafhandelijkeRuimteOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private StandaardOnafhandelijkeRuimteOpbouwMetadata()
			{
			}

			public string SORO_ID { get; set; }

			public string SRO_NR { get; set; }

			public StandaardRuimteOpbouw StandaardRuimteOpbouw { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies StandaardRuimteOpbouwMetadata as the class
	// that carries additional metadata for the StandaardRuimteOpbouw class.
	[MetadataTypeAttribute(typeof(StandaardRuimteOpbouw.StandaardRuimteOpbouwMetadata))]
	public partial class StandaardRuimteOpbouw
	{

		// This class allows you to attach custom attributes to properties
		// of the StandaardRuimteOpbouw class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class StandaardRuimteOpbouwMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private StandaardRuimteOpbouwMetadata()
			{
			}

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }

			public string SRO_ID { get; set; }

			public EntityCollection<StandaardOnafhandelijkeRuimteOpbouw> StandaardOnafhandelijkeRuimteOpbouw { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies SubCatPerPakketMetadata as the class
	// that carries additional metadata for the SubCatPerPakket class.
	[MetadataTypeAttribute(typeof(SubCatPerPakket.SubCatPerPakketMetadata))]
	public partial class SubCatPerPakket
	{

		// This class allows you to attach custom attributes to properties
		// of the SubCatPerPakket class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class SubCatPerPakketMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private SubCatPerPakketMetadata()
			{
			}

			public EntityCollection<NabewerkingSetCombinaties> NabewerkingSetCombinaties { get; set; }

			public string P_NR { get; set; }

			public Pakketten Pakketten { get; set; }

			public string PD_SET_NR { get; set; }

			public ProductSets ProductSets { get; set; }

			public string SCB_NR { get; set; }

			public string SCBP_ID { get; set; }

			public SubCats SubCats { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies SubCatPerRuimteDeelMetadata as the class
	// that carries additional metadata for the SubCatPerRuimteDeel class.
	[MetadataTypeAttribute(typeof(SubCatPerRuimteDeel.SubCatPerRuimteDeelMetadata))]
	public partial class SubCatPerRuimteDeel
	{

		// This class allows you to attach custom attributes to properties
		// of the SubCatPerRuimteDeel class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class SubCatPerRuimteDeelMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private SubCatPerRuimteDeelMetadata()
			{
			}

			public Nullable<decimal> meters { get; set; }

			public string R_NR { get; set; }

			public RuimteDelen RuimteDelen { get; set; }

			public string SCB_NR { get; set; }

			public string SCPR_ID { get; set; }

			public SubCats SubCats { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies SubCatsMetadata as the class
	// that carries additional metadata for the SubCats class.
	[MetadataTypeAttribute(typeof(SubCats.SubCatsMetadata))]
	public partial class SubCats
	{

		// This class allows you to attach custom attributes to properties
		// of the SubCats class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class SubCatsMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private SubCatsMetadata()
			{
			}

			public string C_NR { get; set; }

			public Categorieën Categorieën { get; set; }

			public string eenheidMETERS { get; set; }

			public string Omschrijving { get; set; }

			public string SCB_ID { get; set; }

			public EntityCollection<SubCatPerPakket> SubCatPerPakket { get; set; }

			public EntityCollection<SubCatPerRuimteDeel> SubCatPerRuimteDeel { get; set; }

			public Nullable<int> volgorde { get; set; }
		}
	}
}
