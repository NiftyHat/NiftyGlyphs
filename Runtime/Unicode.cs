using System;
using UnityEngine;

namespace NiftyGlyphs
{
    public static class Unicode
    {
        [Serializable]
        public struct Range
        {
            public int Start => _start;
            public int End => _end;

            [SerializeField][HideInInspector] public string Name;
            [SerializeField] private int _end;
            [SerializeField] private int _start;

            public Range(int start, int end, string name)
            {
                _start = start;
                _end = end;
                Name = name;
            }
        }

        public static class EventCommandNames
        {
            public const string Copy = "Copy";
            public const string Paste = "Paste";
            public const string GlyphBrowserChanged = "GlyphBrowserChanged";
        }

        public static char ReplacementCharacter = Convert.ToChar(0xFFFD);

        public static Range BasicLatin = new Range(0x0020, 0x007F, "Basic Latin");
        public static Range Latin1Supplement = new Range(0x00A0, 0x00FF, "Latin-1 Supplement");
        public static Range LatinExtendedA = new Range(0x0100, 0x017F, "Latin Extended-A");
        public static Range LatinExtendedB = new Range(0x0180, 0x024F, "Latin Extended-B");
        public static Range IPAExtensions = new Range(0x0250, 0x02AF, "IPA Extensions"); //phonetic alphabet.
        public static Range SpacingModifierLetters = new Range(0x02B0, 0x02FF, "Spacing Modifier Letters");
        public static Range CombiningDiacriticalMarks = new Range(0x0300, 0x036F, "Combining Diacritical Marks");

        //languages.
        public static Range GreekandCoptic = new Range(0x0370, 0x03FF, "Greek and Coptic");
        public static Range Cyrillic = new Range(0x0400, 0x04FF, "Cyrillic");
        public static Range CyrillicSupplementary = new Range(0x0500, 0x052F, "Cyrillic Supplementary");
        public static Range Armenian = new Range(0x0530, 0x058F, "Armenian");
        public static Range Hebrew = new Range(0x0590, 0x05FF, "Hebrew");
        public static Range Arabic = new Range(0x0600, 0x06FF, "Arabic");
        public static Range Syriac = new Range(0x0700, 0x074F, "Syriac");
        public static Range Thaana = new Range(0x0780, 0x07BF, "Thaana");
        public static Range Devanagari = new Range(0x0900, 0x097F, "Devanagari");
        public static Range Bengali = new Range(0x0980, 0x09FF, "Bengali");
        public static Range Gurmukhi = new Range(0x0A00, 0x0A7F, "Gurmukhi");
        public static Range Gujarati = new Range(0x0A80, 0x0AFF, "Gujarati");
        public static Range Oriya = new Range(0x0B00, 0x0B7F, "Oriya");
        public static Range Tamil = new Range(0x0B80, 0x0BFF, "Tamil");
        public static Range Telugu = new Range(0x0C00, 0x0C7F, "Telugu");
        public static Range Kannada = new Range(0x0C80, 0x0CFF, "Kannada");
        public static Range Malayalam = new Range(0x0D00, 0x0D7F, "Malayalam");
        public static Range Sinhala = new Range(0x0D80, 0x0DFF, "Sinhala");
        public static Range Thai = new Range(0x0E00, 0x0E7F, "Thai");
        public static Range Lao = new Range(0x0E80, 0x0EFF, "Lao");
        public static Range Tibetan = new Range(0x0F00, 0x0FFF, "Tibetan");
        public static Range Myanmar = new Range(0x1000, 0x109F, "Myanmar");
        public static Range Georgian = new Range(0x10A0, 0x10FF, "Georgian");
        public static Range HangulJamo = new Range(0x1100, 0x11FF, "Hangul Jamo");
        public static Range Ethiopic = new Range(0x1200, 0x137F, "Ethiopic");
        public static Range Cherokee = new Range(0x13A0, 0x13FF, "Cherokee");
        public static Range UnifiedCanadianAboriginalSyllabics =
            new Range(0x1400, 0x167F, "Unified Canadian Aboriginal Syllabics");

        public static Range Ogham = new Range(0x1680, 0x169F, "Ogham");
        public static Range Runic = new Range(0x16A0, 0x16FF, "Runic");
        public static Range Tagalog = new Range(0x1700, 0x171F, "Tagalog");
        public static Range Hanunoo = new Range(0x1720, 0x173F, "Hanunoo");
        public static Range Buhid = new Range(0x1740, 0x175F, "Buhid");
        public static Range Tagbanwa = new Range(0x1760, 0x177F, "Tagbanwa");
        public static Range Khmer = new Range(0x1780, 0x17FF, "Khmer");
        public static Range Mongolian = new Range(0x1800, 0x18AF, "Mongolian");
        public static Range Limbu = new Range(0x1900, 0x194F, "Limbu");
        public static Range TaiLe = new Range(0x1950, 0x197F, "Tai Le");
        public static Range KhmerSymbols = new Range(0x19E0, 0x19FF, "Khmer Symbols");
        public static Range PhoneticExtensions = new Range(0x1D00, 0x1D7F, "Phonetic Extensions");
        public static Range LatinExtendedAdditional = new Range(0x1E00, 0x1EFF, "Latin Extended Additional");
        public static Range GreekExtended = new Range(0x1F00, 0x1FFF, "Greek Extended");
        public static Range GeneralPunctuation = new Range(0x2000, 0x206F, "General Punctuation");
        public static Range SuperscriptsandSubscripts = new Range(0x2070, 0x209F, "Superscripts and Subscripts");
        public static Range CurrencySymbols = new Range(0x20A0, 0x20CF, "Currency Symbols");
        public static Range CombiningDiacriticalMarksforSymbols =
            new Range(0x20D0, 0x20FF, "Combining Diacritical Marks for Symbols");
        public static Range LetterlikeSymbols = new Range(0x2100, 0x214F, "Letterlike Symbols");
        public static Range NumberForms = new Range(0x2150, 0x218F, "Number Forms");
        public static Range Arrows = new Range(0x2190, 0x21FF, "Arrows");
        public static Range MathematicalOperators = new Range(0x2200, 0x22FF, "Mathematical Operators");
        public static Range MiscellaneousTechnical = new Range(0x2300, 0x23FF, "Miscellaneous Technical");
        public static Range ControlPictures = new Range(0x2400, 0x243F, "Control Pictures");
        public static Range OpticalCharacterRecognition = new Range(0x2440, 0x245F, "Optical Character Recognition");
        public static Range EnclosedAlphanumerics = new Range(0x2460, 0x24FF, "Enclosed Alphanumerics");
        public static Range BoxDrawing = new Range(0x2500, 0x257F, "Box Drawing");
        public static Range BlockElements = new Range(0x2580, 0x259F, "Block Elements");
        public static Range GeometricShapes = new Range(0x25A0, 0x25FF, "Geometric Shapes");
        public static Range MiscellaneousSymbols = new Range(0x2600, 0x26FF, "Miscellaneous Symbols");
        public static Range Dingbats = new Range(0x2700, 0x27BF, "Dingbats");
        public static Range MiscellaneousMathematicalSymbolsA =
            new Range(0x27C0, 0x27EF, "Miscellaneous Mathematical Symbols-A");
        public static Range SupplementalArrowsA = new Range(0x27F0, 0x27FF, "Supplemental Arrows-A");
        public static Range BraillePatterns = new Range(0x2800, 0x28FF, "Braille Patterns");
        public static Range SupplementalArrowsB = new Range(0x2900, 0x297F, "Supplemental Arrows-B");
        public static Range MiscellaneousMathematicalSymbolsB =
            new Range(0x2980, 0x29FF, "Miscellaneous Mathematical Symbols-B");
        public static Range SupplementalMathematicalOperators =
            new Range(0x2A00, 0x2AFF, "Supplemental Mathematical Operators");
        public static Range MiscellaneousSymbolsandArrows =
            new Range(0x2B00, 0x2BFF, "Miscellaneous Symbols and Arrows");
        public static Range CJKRadicalsSupplement = new Range(0x2E80, 0x2EFF, "CJK Radicals Supplement");
        public static Range KangxiRadicals = new Range(0x2F00, 0x2FDF, "Kangxi Radicals");
        public static Range IdeographicDescriptionCharacters =
            new Range(0x2FF0, 0x2FFF, "Ideographic Description Characters");
        public static Range CJKSymbolsandPunctuation = new Range(0x3000, 0x303F, "CJK Symbols and Punctuation");
        public static Range Hiragana = new Range(0x3040, 0x309F, "Hiragana");
        public static Range Katakana = new Range(0x30A0, 0x30FF, "Katakana");
        public static Range Bopomofo = new Range(0x3100, 0x312F, "Bopomofo");
        public static Range HangulCompatibilityJamo = new Range(0x3130, 0x318F, "Hangul Compatibility Jamo");
        public static Range Kanbun = new Range(0x3190, 0x319F, "Kanbun");
        public static Range BopomofoExtended = new Range(0x31A0, 0x31BF, "Bopomofo Extended");
        public static Range KatakanaPhoneticExtensions = new Range(0x31F0, 0x31FF, "Katakana Phonetic Extensions");
        public static Range EnclosedCJKLettersandMonths = new Range(0x3200, 0x32FF, "Enclosed CJK Letters and Months");
        public static Range CJKCompatibility = new Range(0x3300, 0x33FF, "CJK Compatibility");
        public static Range CJKUnifiedIdeographsExtensionA =
            new Range(0x3400, 0x4DBF, "CJK Unified Ideographs Extension A");
        public static Range YijingHexagramSymbols = new Range(0x4DC0, 0x4DFF, "Yijing Hexagram Symbols");
        public static Range CJKUnifiedIdeographs = new Range(0x4E00, 0x9FFF, "CJK Unified Ideographs");
        public static Range YiSyllables = new Range(0xA000, 0xA48F, "Yi Syllables");
        public static Range YiRadicals = new Range(0xA490, 0xA4CF, "Yi Radicals");
        public static Range HangulSyllables = new Range(0xAC00, 0xD7AF, "Hangul Syllables");
        public static Range HighSurrogates = new Range(0xD800, 0xDB7F, "High Surrogates");
        public static Range HighPrivateUseSurrogates = new Range(0xDB80, 0xDBFF, "High Private Use Surrogates");
        public static Range LowSurrogates = new Range(0xDC00, 0xDFFF, "Low Surrogates");
        public static Range PrivateUseArea = new Range(0xE000, 0xF8FF, "Private Use Area");
        public static Range CJKCompatibilityIdeographs = new Range(0xF900, 0xFAFF, "CJK Compatibility Ideographs");
        public static Range AlphabeticPresentationForms = new Range(0xFB00, 0xFB4F, "Alphabetic Presentation Forms");
        public static Range ArabicPresentationFormsA = new Range(0xFB50, 0xFDFF, "Arabic Presentation Forms-A");
        public static Range VariationSelectors = new Range(0xFE00, 0xFE0F, "Variation Selectors");
        public static Range CombiningHalfMarks = new Range(0xFE20, 0xFE2F, "Combining Half Marks");
        public static Range CJKCompatibilityForms = new Range(0xFE30, 0xFE4F, "CJK Compatibility Forms");
        public static Range SmallFormVariants = new Range(0xFE50, 0xFE6F, "Small Form Variants");
        public static Range ArabicPresentationFormsB = new Range(0xFE70, 0xFEFF, "Arabic Presentation Forms-B");
        public static Range HalfwidthandFullwidthForms = new Range(0xFF00, 0xFFEF, "Halfwidth and Fullwidth Forms");
        public static Range Specials = new Range(0xFFF0, 0xFFFF, "Specials");
        public static Range LinearBSyllabary = new Range(0x10000,0x1007F,"Linear B Syllabary");
        public static Range LinearBIdeograms = new Range(0x10080,0x100FF,"Linear B Ideograms");
        public static Range AegeanNumbers = new Range(0x10100,0x1013F,"Aegean Numbers");
        public static Range OldItalic = new Range(0x10300,0x1032F,"Old Italic");
        public static Range Gothic = new Range(0x10330,0x1034F,"Gothic");
        public static Range Ugaritic = new Range(0x10380,0x1039F,"Ugaritic");
        public static Range Deseret = new Range(0x10400,0x1044F,"Deseret");
        public static Range Shavian = new Range(0x10450,0x1047F,"Shavian");
        public static Range Osmanya = new Range(0x10480,0x104AF,"Osmanya");
        public static Range CypriotSyllabary = new Range(0x10800,0x1083F,"Cypriot Syllabary");
        public static Range ByzantineMusicalSymbols = new Range(0x1D000,0x1D0FF,"Byzantine Musical Symbols");
        public static Range MusicalSymbols = new Range(0x1D100,0x1D1FF,"Musical Symbols");
        public static Range TaiXuanJingSymbols = new Range(0x1D300,0x1D35F,"Tai Xuan Jing Symbols");
        public static Range MathematicalAlphanumericSymbols = new Range(0x1D400,0x1D7FF,"Mathematical Alphanumeric Symbols");
        public static Range CJKUnifiedIdeographsExtensionB = new Range(0x20000,0x2A6DF,"CJK Unified Ideographs Extension B");
        public static Range CJKCompatibilityIdeographsSupplement = new Range(0x2F800,0x2FA1F,"CJK Compatibility Ideographs Supplement");
        public static Range Tags = new Range(0xE0000,0xE007F,"Tags");
        

        
        public static Range[] AllRanges =
        {
            BasicLatin, Latin1Supplement, LatinExtendedA, LatinExtendedB, IPAExtensions, SpacingModifierLetters,
            CombiningDiacriticalMarks, GreekandCoptic, Cyrillic, CyrillicSupplementary, Armenian, Hebrew, Arabic,
            Syriac, Thaana, Devanagari, Bengali, Gurmukhi, Gujarati, Oriya, Tamil, Telugu, Kannada, Malayalam, Sinhala,
            Thai, Lao, Tibetan, Myanmar, Georgian, HangulJamo, Ethiopic, Cherokee, UnifiedCanadianAboriginalSyllabics,
            Ogham, Runic, Tagalog, Hanunoo, Buhid, Tagbanwa, Khmer, Mongolian, Limbu, TaiLe, KhmerSymbols,
            PhoneticExtensions, LatinExtendedAdditional, GreekExtended, GeneralPunctuation, SuperscriptsandSubscripts,
            CurrencySymbols, CombiningDiacriticalMarksforSymbols, LetterlikeSymbols, NumberForms, Arrows,
            MathematicalOperators, MiscellaneousTechnical, ControlPictures, OpticalCharacterRecognition,
            EnclosedAlphanumerics, BoxDrawing, BlockElements, GeometricShapes, MiscellaneousSymbols, Dingbats,
            MiscellaneousMathematicalSymbolsA, SupplementalArrowsA, BraillePatterns, SupplementalArrowsB,
            MiscellaneousMathematicalSymbolsB, SupplementalMathematicalOperators, MiscellaneousSymbolsandArrows,
            CJKRadicalsSupplement, KangxiRadicals, IdeographicDescriptionCharacters, CJKSymbolsandPunctuation, Hiragana,
            Katakana, Bopomofo, HangulCompatibilityJamo, Kanbun, BopomofoExtended, KatakanaPhoneticExtensions,
            EnclosedCJKLettersandMonths, CJKCompatibility, CJKUnifiedIdeographsExtensionA, YijingHexagramSymbols,
            CJKUnifiedIdeographs, YiSyllables, YiRadicals, HangulSyllables, HighSurrogates, HighPrivateUseSurrogates,
            LowSurrogates, PrivateUseArea, CJKCompatibilityIdeographs, AlphabeticPresentationForms,
            ArabicPresentationFormsA, VariationSelectors, CombiningHalfMarks, CJKCompatibilityForms, SmallFormVariants,
            ArabicPresentationFormsB, HalfwidthandFullwidthForms, Specials, LinearBSyllabary, LinearBIdeograms,
            AegeanNumbers, OldItalic, Gothic, Ugaritic, Deseret, Shavian, Osmanya, CypriotSyllabary,
            ByzantineMusicalSymbols, MusicalSymbols, TaiXuanJingSymbols, MathematicalAlphanumericSymbols,
            CJKUnifiedIdeographsExtensionB, CJKCompatibilityIdeographsSupplement, Tags
        };

        public static Range[] InterfaceRanges =
        {
            MiscellaneousSymbols,
            GeometricShapes,
            BoxDrawing,
            Arrows
        };

        public static Range[] SymbolsRanges =
        {
            MiscellaneousSymbolsandArrows,
            SupplementalArrowsB,
            SupplementalArrowsA,
            MiscellaneousMathematicalSymbolsA,
            MiscellaneousMathematicalSymbolsB,
            Dingbats,
            MiscellaneousSymbols,
            GeometricShapes,
            BoxDrawing,
            Arrows,
            LetterlikeSymbols,
            CurrencySymbols
        };
        
        public static Range[] LinearBRanges =
        {
            LinearBSyllabary,
            LinearBIdeograms
        };
        
        public static Range[] LatinRanges =
            { BasicLatin, Latin1Supplement, LatinExtendedA, LatinExtendedB, LatinExtendedAdditional };
        
        public static Range[] CyrillicRanges =
            { Cyrillic, CyrillicSupplementary};

        public static Range[] GreekRanges =
        {
            GreekandCoptic, GreekExtended
        };
        
        public static Range[] MathsRanges =
        {
            MathematicalOperators, SupplementalMathematicalOperators, MiscellaneousMathematicalSymbolsA, MiscellaneousMathematicalSymbolsB
        };
    }
}