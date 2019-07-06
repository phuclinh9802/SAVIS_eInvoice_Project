using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    public partial class Utils
    {
        //hoant: from Full name => Last name and first name
        public static string[] SplitFullname(string fullName)
        {
            // [0] FirstName, [1] LastName
            string[] result = new string[2];
            if (string.IsNullOrEmpty(fullName)) return new string[0];
            string[] partOfName = fullName.Split(' ');
            if (partOfName.Length == 1)
            {
                result[0] = partOfName[0];
                result[1] = partOfName[0];
            }
            else if (partOfName.Length >= 2)
            {
                result[0] = partOfName.Last();
                result[1] = string.Join(" ", partOfName.Where((val, idx) => idx != Array.IndexOf(partOfName, partOfName.Last())).ToArray());
            }
            return result;
        }
        public static bool IsValidJsonObject(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
        public static bool IsValidJsonArray(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

        public static string ReOrderPemission(string permissionString)
        {

            var listPermission = permissionString.ToList();
            var nuberPermission = new List<ObjectScore>();
            foreach (var item in listPermission)
            {
                switch (item)
                {
                    case 'r':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'w':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'e':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'd':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'f':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    default:
                        break;
                }
            }
            nuberPermission.OrderBy(s => s.Score);
            return string.Join("", nuberPermission.Select(s => s.Value.ToString()));
        }
        private static readonly string[] VietnameseSigns = new string[]
        {
        "aAeEoOuUiIdDyY-",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ",
        " "
        };
        public static string RemoveVietnameseSign(string str)
        {

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;

        }
        public static string BuildVietnameseSign(string str)
        {

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[0][i - 1], VietnameseSigns[i][j]);

            }

            return str;

        }
        public static List<Guid> GetListIdFromCommaString(string listId)
        {
            var listIdObj = new List<Guid>();

            var spilit = listId.Split(',');

            foreach (var item in spilit)
            {
                try
                {
                    Guid newGuid = Guid.Parse(item);
                    listIdObj.Add(newGuid);
                }
                catch (Exception)
                {
                    return new List<Guid>();
                }
            }

            return listIdObj;

        }
        /// <summary>
        /// Convert url title
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConvertToUrlTitle(string name)
        {
            string strNewName = name;

            #region Replace unicode chars
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = name.Normalize(NormalizationForm.FormD);
            strNewName = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            #endregion

            #region Replace special chars
            string strSpecialString = "~\"“”#%&*:;<>?/\\{|}.+_@$^()[]`,!-'";

            foreach (char c in strSpecialString.ToCharArray())
            {
                strNewName = strNewName.Replace(c, ' ');
            }
            #endregion

            #region Replace space

            // Create the Regex.
            var r = new Regex(@"\s+");
            // Strip multiple spaces.
            strNewName = r.Replace(strNewName, @" ").Replace(" ", "-").Trim('-');

            #endregion)

            return strNewName;
        }
        /// <summary>
        /// Check if a string is a guid or not
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool IsGuid(string inputString)
        {
            try
            {
                var guid = new Guid(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsNumber(string inputString)
        {
            try
            {
                var guid = int.Parse(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GeneratePageUrl(string pageTitle)
        {
            var result = RemoveVietnameseSign(pageTitle);

            // Replace spaces
            result = result.Replace(" ", "-");

            // Replace double spaces
            result = result.Replace("--", "-");

            // Remove triple spaces
            result = result.Replace("---", "-");

            return result;

        }
        /// <summary>
        /// Tạo chuỗi 6 chữ số
        /// </summary>
        /// <returns></returns>
        public static string GenerateNewRandom()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateNewRandom();
            }
            return r;
        }
        public static string PasswordRandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public static string PasswordCreateSalt512()
        {
            var message = PasswordRandomString(512, false);
            return BitConverter.ToString((new SHA512Managed()).ComputeHash(Encoding.ASCII.GetBytes(message))).Replace("-", "");
        }
        public static string RandomPassword(int numericLength, int lCaseLength, int uCaseLength, int specialLength)
        {
            Random random = new Random();

            //char set random
            string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string PASSWORD_CHARS_NUMERIC = "1234567890";
            string PASSWORD_CHARS_SPECIAL = "!@#$%^&*()-+<>?";
            if ((numericLength + lCaseLength + uCaseLength + specialLength) < 8)
                return string.Empty;
            else
            {
                //get char
                var strNumeric = new string(Enumerable.Repeat(PASSWORD_CHARS_NUMERIC, numericLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strUper = new string(Enumerable.Repeat(PASSWORD_CHARS_UCASE, uCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strSpecial = new string(Enumerable.Repeat(PASSWORD_CHARS_SPECIAL, specialLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strLower = new string(Enumerable.Repeat(PASSWORD_CHARS_LCASE, lCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                //result : ký tự số + chữ hoa + chữ thường + các ký tự đặc biệt > 8
                var strResult = strNumeric + strUper + strSpecial + strLower;
                return strResult;
            }
        }
        public static string PasswordGenerateHmac(string clearMessage, string secretKeyString)
        {
            var encoder = new ASCIIEncoding();
            var messageBytes = encoder.GetBytes(clearMessage);
            var secretKeyBytes = new byte[secretKeyString.Length / 2];
            for (int index = 0; index < secretKeyBytes.Length; index++)
            {
                string byteValue = secretKeyString.Substring(index * 2, 2);
                secretKeyBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            var hmacsha512 = new HMACSHA512(secretKeyBytes);
            byte[] hashValue = hmacsha512.ComputeHash(messageBytes);
            string hmac = "";
            foreach (byte x in hashValue)
            {
                hmac += String.Format("{0:x2}", x);
            }
            return hmac.ToUpper();
        }
        public static Expression<Func<T, bool>> PredicateByName<T>(string propName, object propValue)
        {
            var parameterExpression = Expression.Parameter(typeof(T));
            var propertyOrField = Expression.PropertyOrField(parameterExpression, propName);
            var binaryExpression = Expression.GreaterThan(propertyOrField, Expression.Constant(propValue));
            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
        }

        //public static string TikaExtractor(string filePath)
        //{
        //    var textExtractor = new TextExtractor();

        //    return textExtractor.Extract(filePath).Text;
        //}
        //public static string TikaExtractorFromUe(Uri uri)
        //{
        //    var textExtractor = new TextExtractor();
        //    return textExtractor.Extract(uri).Text;
        //}

        private static string[] ChuSo = new string[10] { " không", " một", " hai", " ba", " bốn", " năm", " sáu", " bẩy", " tám", " chín" };
        private static string[] Tien = new string[6] { "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ" };
        // Hàm đọc số thành chữ

        public static string DocTienBangChu(long SoTien, string strTail)
        {
            int lan, i;
            long so;
            string KetQua = "", tmp = "";
            int[] ViTri = new int[6];
            if (SoTien < 0) return "Số tiền âm !";
            if (SoTien == 0) return "Không đồng !";
            if (SoTien > 0)
            {
                so = SoTien;
            }
            else
            {
                so = -SoTien;
            }
            //Kiểm tra số quá lớn
            if (SoTien > 8999999999999999)
            {
                SoTien = 0;
                return "";
            }
            ViTri[5] = (int)(so / 1000000000000000);
            so = so - long.Parse(ViTri[5].ToString()) * 1000000000000000;
            ViTri[4] = (int)(so / 1000000000000);
            so = so - long.Parse(ViTri[4].ToString()) * 1000000000000;
            ViTri[3] = (int)(so / 1000000000);
            so = so - long.Parse(ViTri[3].ToString()) * 1000000000;
            ViTri[2] = (int)(so / 1000000);
            ViTri[1] = (int)((so % 1000000) / 1000);
            ViTri[0] = (int)(so % 1000);
            if (ViTri[5] > 0)
            {
                lan = 5;
            }
            else if (ViTri[4] > 0)
            {
                lan = 4;
            }
            else if (ViTri[3] > 0)
            {
                lan = 3;
            }
            else if (ViTri[2] > 0)
            {
                lan = 2;
            }
            else if (ViTri[1] > 0)
            {
                lan = 1;
            }
            else
            {
                lan = 0;
            }
            for (i = lan; i >= 0; i--)
            {
                tmp = DocSo3ChuSo(ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] != 0) KetQua += Tien[i];
                if ((i > 0) && (!string.IsNullOrEmpty(tmp))) KetQua += ",";//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.Substring(KetQua.Length - 1, 1) == ",") KetQua = KetQua.Substring(0, KetQua.Length - 1);
            KetQua = KetQua.Trim() + strTail;
            return KetQua.Substring(0, 1).ToUpper() + KetQua.Substring(1);
        }
        // Hàm đọc số có 3 chữ số
        private static string DocSo3ChuSo(int baso)
        {
            int tram, chuc, donvi;
            string KetQua = "";
            tram = (int)(baso / 100);
            chuc = (int)((baso % 100) / 10);
            donvi = baso % 10;
            if ((tram == 0) && (chuc == 0) && (donvi == 0)) return "";
            if (tram != 0)
            {
                KetQua += ChuSo[tram] + " trăm";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh";
            }
            if ((chuc != 0) && (chuc != 1))
            {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh";
            }
            if (chuc == 1) KetQua += " mười";
            switch (donvi)
            {
                case 1:
                    if ((chuc != 0) && (chuc != 1))
                    {
                        KetQua += " mốt";
                    }
                    else
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    else
                    {
                        KetQua += " lăm";
                    }
                    break;
                default:
                    if (donvi != 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }

        public static string ReplaceMailBodyParam(string content, string tenkhachhang, string makhachhang, string masothue, string sohoadon, string mausohoadon, string kyhieuhoadon, string matracuu, string tentaikhoan, string matkhau, string linktracuu)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            return content.Replace(MailConstants.MailBodyParam.TEN_KHACH_HANG, tenkhachhang).Replace(MailConstants.MailBodyParam.MA_KHACH_HANG, makhachhang).Replace(MailConstants.MailBodyParam.MA_SO_THUE, masothue).Replace(MailConstants.MailBodyParam.SO_HOA_DON, sohoadon).Replace(MailConstants.MailBodyParam.MAU_SO_HOA_DON, mausohoadon).Replace(MailConstants.MailBodyParam.KY_HIEU_HOA_DON, kyhieuhoadon).Replace(MailConstants.MailBodyParam.TEN_TAI_KHOAN, tentaikhoan).Replace(MailConstants.MailBodyParam.MAT_KHAU, matkhau).Replace(MailConstants.MailBodyParam.MA_TRA_CUU, matracuu).Replace(MailConstants.MailBodyParam.LINK_TRA_CUU_HOA_DON, linktracuu);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static DateTime TryToConvertDateTimeFromString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime psrDate = DateTime.Now;
                string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy h:mm tt", "dd/MM/yyyy h:mm:ss tt", "dd-MM-yyyy h:mm tt", "dd-MM-yyyy h:mm:ss tt", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm", "dd-MMM-yyyy HH:mm:ss", "dd-MMM-yyyy", "dd-MMM-yy", "dd-MMM-yy HH:mm", "dd-MMM-yy HH:mm:ss" };
                if (DateTime.TryParseExact(value, formats, null, System.Globalization.DateTimeStyles.None, out psrDate))
                {
                    return psrDate;
                }
            }
            return DateTime.MinValue;
        }

        public class ObjectScore
        {
            public object Value { get; set; }
            public int Score { get; set; }
        }
    }
}
