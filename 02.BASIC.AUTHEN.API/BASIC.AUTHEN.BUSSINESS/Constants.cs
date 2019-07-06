using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    public class MessageResponseConstants
    {
        public const string MESSAGE_ADD_SUCCESS = "Tạo mới thành công";
        public const string MESSAGE_ADD_FAILED_DB = "Tạo mới thất bại. Lỗi khi thực hiện câu lệnh SQL";
        public const string MESSAGE_UPDATE_SUCCESS = "Cập nhật thành công";
        public const string MESSAGE_UPDATE_FAILED_DB = "Cập nhật thất bại. Lỗi khi thực hiện câu lệnh SQL";
        public const string MESSAGE_DELETE_SUCCESS = "Xóa thành công";
        public const string MESSAGE_DELETE_FAILED_DB = "Xóa thất bại. Lỗi khi thực hiện câu lệnh SQL";
        public const string MESSAGE_DELETE_CANNOT_DELETE = "Xóa thất bại. Không có quyền xóa bản ghi này";
        public const string MESSAGE_EXCEPTION = "Có lỗi ngoại lệ xảy ra: ";
        public const string MESSAGE_GET_SUCCESS = "Lấy dữ liệu thành công";
        public const string MESSAGE_GET_NOT_FOUND_SUCCESS = "Không tìm thấy dữ liệu cần xử lý";

        public class InvoiceMessage
        {
            public const string MESSAGE_ORIGINAL_INVOICE_NOT_FOUND = "Không tìm thấy thông tin hóa đơn gốc";
            public const string MESSAGE_ORIGINAL_INVOICE_REPLACED_BY_OTHER_INVOICE = "Hóa đơn gốc đã bị thay thế bởi hóa đơn khác";
            public const string MESSAGE_INVOICE_PUBLISH_FAILED = "Không thể phát hành hóa đơn này";
            public const string MESSAGE_INVOICE_REPLACE_FAILED = "Không thể thay thế hóa đơn này";
            public const string MESSAGE_INVOICE_ADJUSTMENT_FAILED = "Không thể điều chỉnh hóa đơn này";
            public const string MESSAGE_INVOICE_CANCEL_FAILED = "Không thể xóa bỏ hóa đơn này";
            public const string MESSAGE_PUBLISH_SUCCESS = "Phát hành hóa đơn thành công";
            public const string MESSAGE_PUBLISH_FAILED_DB = "Phát hành hóa đơn thất bại. Lỗi khi thực hiện câu lệnh SQL";
            public const string MESSAGE_REPLACE_SUCCESS = "Thay thế hóa đơn thành công";
            public const string MESSAGE_REPLACE_FAILED_DB = "Thay thế hóa đơn thất bại. Lỗi khi thực hiện câu lệnh SQL";
            public const string MESSAGE_ADJUSTMENT_SUCCESS = "Điều chỉnh hóa đơn thành công";
            public const string MESSAGE_ADJUSTMENT_FAILED_DB = "Điều chỉnh hóa đơn thất bại. Lỗi khi thực hiện câu lệnh SQL";
            public const string MESSAGE_CANCEL_SUCCESS = "Xóa bỏ hóa đơn thành công";
            public const string MESSAGE_CANCEL_FAILED_DB = "Xóa bỏ hóa đơn thất bại. Lỗi khi thực hiện câu lệnh SQL";
        }
    }
    public class MailConstants
    {
        public class MailConfigParameter
        {
            public const string MAIL_CONFIG_ENABLED = "email:enabled";
            public const string MAIL_CONFIG_PORT = "email:port";
            public const string MAIL_CONFIG_FROM = "email:from";
            public const string MAIL_CONFIG_SMTP = "email:smtp";
            public const string MAIL_CONFIG_SSL = "email:ssl";
            public const string MAIL_CONFIG_PASSWORD = "email:password";
            public const string MAIL_CONFIG_USER = "email:user";
            public const string MAIL_CONFIG_SEND_TYPE = "email:sendtype";
        }
        public class MailConfigSendType
        {
            public const string ASYNC = "async";
            public const string SYNC = "sync";
            public const string BOTH = "both";
        }
        public class MailInvoiceType
        {
            public const string MAIL_HOA_DON_THONG_THUONG = "1";
            public const string MAIL_HOA_DON_THAY_THE = "2";
            public const string MAIL_HOA_DON_DIEU_CHINH = "3";
            public const string MAIL_HOA_DON_HUY = "4";
            public const string MAIL_THONG_BAO_TAI_KHOAN_KHACH_HANG = "5";
        }
        public class MailBodyParam
        {
            public const string TEN_KHACH_HANG = "&amp;TEN_KHACH_HANG&amp;";
            public const string MA_KHACH_HANG = "&amp;MA_KHACH_HANG&amp;";
            public const string MA_SO_THUE = "&amp;MA_SO_THUE&amp;";
            public const string SO_HOA_DON = "&amp;SO_HOA_DON&amp;";
            public const string MAU_SO_HOA_DON = "&amp;MAU_SO_HOA_DON&amp;";
            public const string KY_HIEU_HOA_DON = "&amp;KY_HIEU_HOA_DON&amp;";
            public const string MA_TRA_CUU = "&amp;MA_TRA_CUU&amp;";
            public const string TEN_TAI_KHOAN = "&amp;TEN_TAI_KHOAN&amp;";
            public const string MAT_KHAU = "&amp;MAT_KHAU&amp;";
            public const string LINK_TRA_CUU_HOA_DON = "&amp;LINK_TRA_CUU_HD&amp;";
        }
        public const string EMAIL_REGEX_PATTERN = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public const char EMAIL_SPLIT_CHARACTER = ';';
    }
    public class DateTimeFormatConstants
    {
        public const string DDMMYYYY = "dd/MM/yyyy";
        public const string YYMMDDHHMMSSFFFF = "yyyyMMddHHmmssffff";
    }
    public class CONSTANT_APP
    {
        public const string ADMIN_ID = "2fb87188-35b5-48d1-b527-58bf3a2b12b2";
    }

    public class CONSTANT_CATALOG
    {
        public const string HINH_THUC_THANH_TOAN = "danh-muc-hinh-thuc-thanh-toan";
        public const string DON_VI_TINH = "danh-muc-don-vi-tinh";
    }

    //Dùng khi phát hành hóa đơn và PMKT gọi dịch vụ UpdateStatusInvoice
    public class CONSTANT_STATUS_GIVE
    {
        public const int CHUA_GUI = 0;//PM HĐĐT chưa gửi sang PMKT
        public const int DA_GUI = 1;//PM HĐĐT đã gửi sang PMKT và PMKT chưa nhận được hoặc lưu
        public const int DA_NHAN = 2;//PMKT đã nhận và đã lưu
    }

    public class CONSTANT_STATUS_API
    {
        public const int THANH_CONG = 1;
        public const int THAT_BAI = 0;
    }

    public class InvoiceConstants
    {
        public const string PREFIX_INVOICE_CODE = "HD";

        public class InvoiceStatus
        {
            public const int HOA_DON_MOI_TAO_LAP = 1;
            public const int HOA_DON_DA_PHAT_HANH = 2;
            public const int HOA_DON_BI_THAY_THE = 3;
            public const int HOA_DON_BI_DIEU_CHINH = 4;
            public const int HOA_DON_BI_XOA_BO = 5;
        }

        public class InvoiceAdjustmentType
        {
            public const int HOA_DON_THONG_THUONG = 1;
            public const int HOA_DON_THAY_THE = 2;
            public const int HOA_DON_DIEU_CHINH_TANG = 3;
            public const int HOA_DON_DIEU_CHINH_GIAM = 4;
            public const int HOA_DON_DIEU_CHINH_THONG_TIN = 5;
        }

        public class InvoiceAdjustmentTypeXml
        {
            public const int HOA_DON_THUONG = 1;
            public const int HOA_DON_BI_THAY_THE = 2;
            public const int HOA_DON_THAY_THE = 3;
            public const int HOA_DON_BI_DIEU_CHINH = 4;
            public const int HOA_DON_DIEU_CHINH = 5;
            public const int HOA_DON_BI_XOA_BO = 6;
            public const int HOA_DON_XOA_BO = 7;
            public const int HOA_DON_HUY = 8;
            public const int HOA_DON_DIEU_CHINH_CHIET_KHAU = 9;
        }

        public const string FIRST_INVOICE_NUMBER = "0000000";

        public class TaxRate
        {
            public const decimal KHONG_PHAN_TRAM = 0;
            public const decimal NAM_PHAN_TRAM = 5;
            public const decimal MUOI_PHAN_TRAM = 10;
        }

        public class TaxRateName
        {
            public const string KHONG_THUE = "1. Hàng hóa, dịch vụ không chịu thuế giá trị gia tăng (GTGT):";
            public const string KHONG_PHAN_TRAM = "2. Hàng hóa, dịch vụ chịu thuế suất thuế GTGT 0%:";
            public const string NAM_PHAN_TRAM = "3. Hàng hóa, dịch vụ chịu thuế suất thuế GTGT 5%:";
            public const string MUOI_PHAN_TRAM = "4. Hàng hóa, dịch vụ chịu thuế suất thuế GTGT 10%:";
        }

        public class InvoiceType
        {
            public const string HOA_DON_GIA_TRI_GIA_TANG = "01GTKT";
            public const string HOA_DON_BAN_HANG = "02GTTT";
            public const string HOA_DON_BAN_HANG_PHI_THUE_QUAN = "07KPTQ";
            public const string PHIEU_XUAT_KHO_VAN_CHUYEN_NOI_BO = "03XKNB";
            public const string PHIEU_XUAT_KHO_GUI_BAN_HANG_DAI_LY = "04HGDL";
        }

        public class VatPercentageType
        {
            public const int KHONG_PHAI_KE_KHAI_TINH_THUE = -2;
            public const int KHONG_CHIU_THUE = -1;
            public const int THUE_SUAT_0 = 0;
            public const int THUE_SUAT_5 = 5;
            public const int THUE_SUAT_10 = 10;
        }

        public class ProductInvoiceType
        {
            public const int HANG_HOA_BINH_THUONG = 0;
            public const int DIEU_CHINH_GIAM_GIA_HANG_CHIET_KHAU_KHUYEN_MAI = 1;
            public const int HANG_KHUYEN_MAI_TANG_KHONG_TINH_TIEN = 2;
        }
    }
}
