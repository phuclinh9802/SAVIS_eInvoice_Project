# Project: eInvoice.ERP

#### IDE

* [License Visual Studio](https://gist.github.com/CHEF-KOCH/6f619cc1bc905d12917643c3a6a168a1)
* [License Reshaper](https://thepiratebay.org/torrent/13275860/%5BCrack%5D_JetBrains_License_Server_v1.1)
* [License Reshaper Server Online](http://xidea.online/servers.html)

#### FixMissing Depency

> Update-Package

> Update-Package -Project YourProjectName

> Update-Package -reinstall

> Update-Package -reinstall -Project YourProjectName

# Entity-Framework function

#### Tạo model từ một database có sẵn

> Scaffold-DbContext "Server=192.168.222.129;initial catalog=eInvoice.CoreFW.ERP;Uid=sa;Pwd=1Qaz2wsx;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models

> Scaffold-DbContext "Server=192.168.222.129:1521/xe;PASSWORD=1Qaz2wsx;PERSIST SECURITY INFO=True;USER ID=eInvoice.CoreFW.ERP;" Devart.Data.Oracle.Entity.EFCore  -OutputDir Models

> dotnet ef --startup-project ..\eInvoice.ERP.API\  dbcontext scaffold "Server=192.168.0.33;initial catalog=eInvoice.ERP.Solution;Uid=sa;Pwd=1Qaz2wsx;" Microsoft.EntityFrameworkCore.SqlServer -c DataContext -o V1/EFDataAccess -d
 

#### Tạo một db changelog
 
> Add-Migration InitDatabase -verbose -Context eInvoice.ERP.Data_V1.DataContext -OutputDir V1/Migrations
 
 
> dotnet ef --startup-project ..\eInvoice.ERP.API\ migrations add <MigateName>
 

#### Cập nhập db changelog vào bảng
 
> Update-Database -verbose
 
 
>  dotnet ef --startup-project ..\eInvoice.ERP.API\ database update
#### Sinh script cập nhật
 
> Script-Migration -verbose
 
 
>  dotnet ef --startup-project ..\eInvoice.ERP.API\ migrations script  --output "../Docker.oracle/Script-Migration/2.MigrateScript.sql"  
# Document

# Technology

* [LinqKit](https://github.com/scottksmith95/LINQKit)
* [AutoMapper](https://github.com/AutoMapper/AutoMapper)

# Best Practices
 
### Chuẩn bị sẵn dữ liệu khi dùng linq query table

--> Bad

```sh
 var query = from rmu in unitOfWork.GetRepository<IdmRight_Map_User>().GetAll()
                              where listUserId.Contains(rmu.UserId) && rmu.InheritedFromRoles.Contains(roleId.ToString())  
                              select rmu;
```

--> Good

```sh
var roleIdString = roleId.ToString();
 var query = from rmu in unitOfWork.GetRepository<IdmRight_Map_User>().GetAll()
                              where listUserId.Contains(rmu.UserId) && rmu.InheritedFromRoles.Contains(roleIdString)  
                              select rmu;
```

### Sử dụng câu lệnh LIKE trong SQL

> LIKE 'a%' => StartsWith("a")

> LIKE '%a' => EndsWith("a")

> LIKE '%a%' => Contains("a")

> LIKE 'a%b' => StartsWith("a") && EndsWith("b")

> LIKE '%a%b%' => StartsWith("a") && Contains("b")

### Set charset cho MySQL database

```sh
 protected override void Up(MigrationBuilder migrationBuilder)
 {
   migrationBuilder.Sql($@"ALTER DATABASE eInvoice.CoreFW.ERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
 }
```
### Template http://petstore.swagger.io/
### Mã lỗi và quy tắc đặt tên
#### Convention

**Query parameter:**  dùng camel Case, ví dụ: provinceId, districtId

**Field trong request:** 	page, size, provinceId

**Field mặc định trong response có phân trang (pagination):**
1.	code
2.	message
3.	data 
*	**totalPages** : tổng số page toàn hệ thống
*	**size** :số lượng bản ghi lấy ra trên 1 trang
*	**numbefOfElements** : tổng số bản ghi lấy ra 
*	**totalElements**  : tổng số bản ghi toàn hệ thống
*	**content** : danh sách bản ghi

**Field mặc định trong response không có phân trang (listing)**
1.	code
2.	message
3.	data: dạng List<> **\[ \]**

**Field mặc định trong response của api cho entity**
1.	code
2.	detail 
3.	data: dạng Object **\{ \}**

**Các field mặc định trong data**
1.	modified: ngày tháng cập nhật cuối cùng
2.	modifier: tên username cập nhật cuối
3.	Id -> id (để ít phải sửa UI)
4.	wardName -> name (để ít phải sửa UI)
5.	wardCode -> code (để ít phải sửa UI)

**Quy tắc đặt cho database**
1.	Các primary key đặt là Id
2.	Các foreign key đặt theo tên bảng + Id, ví dụ WardId
________________________________________
#### Dữ liệu của các API

**GET** Request
1.	page
2.	size
3.	filter: JSON { wardName: 'hanoi' } / Client: json.stringify (tiêu chí sắp xếp, lọc nâng cao)
4.	sort: JSON Array [modified_asc, name_desc]
5.  
**Ví dụ**
> /wards: trả 20 bản ghi đầu tiên theo tiêu chí sắp xếp mặc định

> /wards?size=10: trả 10 bản ghi đầu tiên theo tiêu chí sắp xếp mặc định

> /wards?size=10&page=2: trả 10 bản ghi của trang số 2 theo tiêu chí sắp xếp mặc định

**GET** Response
*	code
*	message
*	data

**POST** Response
*	code
*	message
*	data

**PUT** Response
*	code
*	message
*	data
    *	id

**DELETE** Response
*	code
*	message
*	data
    *	id
    *	name

**MULTI DELETE** Response
*	code
*	message
*	data: List\<DeleteResponse\>



# Lấy về theo bộ lọc
```sh
query getPosition($filter: PositionInputFilterObject!){
  position_filter (filter:$filter)
  {
    id,
    name,
    note,
    status,
    code
  }
}
```

```sh
Query
{
  "filter": { 
    "TextSearch": "",
    "PageSize": "10",
    "PageNumber": "1",  
  }
}
```
# Lấy về tất cả
```sh
query getAllPosition{
  position
  {
    id,
    name,
    note,
    status,
    code
  }
}
```

# Thêm mới
```sh
mutation createPosition($position: PositionInputCreateObject!) {
  createPosition(position:$position)
  {
    id,
    name
  }
}
```

```sh
>>Query
{
    "position": { 
    "name": "Chuc vu test",
    "code": "CHUC_VU_TEST",
    "note": "note go here", 
    "order": 1, 
  }
}
```

# Cập nhật
```sh
mutation updatePosition($position: PositionInputUpdateObject!) {
  deletePosition(position:$position)
  {
    id,
    name
  }
}
```

```sh
>>Query
{
  "position": {
    "id": "4ff9b46f-6dbb-434c-803e-ecb83b5b0b0b",
    "name": "Chuc vu test updated",
    "code": "CHUC_VU_TEST",
    "note": "note go here",
    "status": true,
    "order": 1
  }
}
```


# Xóa một

```sh
mutation deletePosition  {
  deletePosition(positionId: "4ff9b46f-6dbb-434c-803e-ecb83b5b0b0b")
  {
    positionId,
    name,
    message,
    result
  }
}
```

# Xóa nhiều
```sh
mutation deleteMultiPosition  {
  deleteMultiPosition(listPositionId: ["4ff9b46f-6dbb-434c-803e-ecb83b5b0b0b"])
  {
    positionId,
    name,
    message,
    result
  }
}
```
