# API Docs

Base URL: `/api/v1`

## 1) Create User
- **Method/Path**: `POST /users`
- **Input (JSON body)**:
```json
{
  "firstname": "John",
  "lastname": "Doe",
  "email": "john.doe@example.com",
  "phonenumber": "0901234567",
  "dateOfBirth": "1998-05-20"
}
```
- **Output**:
  - `201 Created`: tạo user thành công
  - `400 Bad Request`: thiếu/sai định dạng input
  - `409 Conflict`: vi phạm business rule (ví dụ dữ liệu đã tồn tại)
  - `502 Bad Gateway`: lỗi gọi external service
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (201)**:
```json
{
  "success": true,
  "message": "User created successfully.",
  "data": {
    "id": 1,
    "firstname": "John",
    "lastname": "Doe",
    "email": "john.doe@example.com",
    "phonenumber": "0901234567",
    "dateOfBirth": "1998-05-20",
    "createdAt": "2026-04-22T12:00:00Z",
    "updatedAt": "2026-04-22T12:00:00Z"
  }
}
```
- **Response mẫu (400)**:
```json
{
  "success": false,
  "message": "Validation failed.",
  "data": {
    "Email": [
      "'Email' is not a valid email address."
    ]
  }
}
```

## 2) Get User Profile
- **Method/Path**: `GET /users/user_profile`
- **Auth**: `Bearer JWT` (header `Authorization: Bearer <token>`)
- **Input**: không có body
- **Output**:
  - `200 OK`: lấy profile thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "User profile fetched successfully.",
  "data": {
    "id": 1,
    "firstname": "John",
    "lastname": "Doe",
    "email": "john.doe@example.com",
    "avatarUrl": "http://localhost:9000/app-bucket/avatars/1/abcd1234....png",
    "phonenumber": "0901234567",
    "dateOfBirth": "1998-05-20"
  }
}
```
- **Response mẫu (401)**:
```json
{
  "success": false,
  "message": "Token is invalid.",
  "data": null
}
```

## 3) Upload User Avatar
- **Method/Path**: `POST /users/avatar`
- **Auth**: `Bearer JWT` (header `Authorization: Bearer <token>`)
- **Input (`multipart/form-data`)**:
  - `avatar` (file) - bắt buộc, hỗ trợ `image/jpeg`, `image/png`, `image/webp`
- **Output**:
  - `200 OK`: upload avatar thành công
  - `400 Bad Request`: thiếu file / sai định dạng / quá dung lượng
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "User avatar uploaded successfully.",
  "data": {
    "id": 12,
    "objectKey": "avatars/1/2f52e7...d8a9.png",
    "url": "http://localhost:9000/app-bucket/avatars/1/2f52e7...d8a9.png",
    "fileName": "avatar.png",
    "contentType": "image/png",
    "size": 48213,
    "createdAt": "2026-04-22T13:00:00Z"
  }
}
```
- **Response mẫu (400)**:
```json
{
  "success": false,
  "message": "Validation failed.",
  "data": {
    "Avatar.ContentType": [
      "Avatar content type is not supported."
    ]
  }
}
```

## 4) Edit User Avatar
- **Method/Path**: `PUT /users/avatar`
- **Auth**: `Bearer JWT` (header `Authorization: Bearer <token>`)
- **Input (`multipart/form-data`)**:
  - `avatar` (file) - bắt buộc
- **Output**:
  - `200 OK`: cập nhật avatar thành công
  - `400 Bad Request`: thiếu file / sai định dạng / quá dung lượng
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "User avatar updated successfully.",
  "data": {
    "id": 13,
    "objectKey": "avatars/1/9c1aef...410b.png",
    "url": "http://localhost:9000/app-bucket/avatars/1/9c1aef...410b.png",
    "fileName": "avatar.png",
    "contentType": "image/png",
    "size": 50120,
    "createdAt": "2026-04-22T13:15:00Z"
  }
}
```

## 5) Delete User Avatar
- **Method/Path**: `DELETE /users/avatar`
- **Auth**: `Bearer JWT` (header `Authorization: Bearer <token>`)
- **Input**: không có body
- **Output**:
  - `200 OK`: xóa avatar thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "User avatar deleted successfully.",
  "data": {
    "deleted": true
  }
}
```

## 6) Create Branch
- **Method/Path**: `POST /branches`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input (JSON body)**:
```json
{
  "name": "Branch HCM 01",
  "description": "Main campus in Ho Chi Minh city",
  "addressLine1": "12 Nguyen Trai",
  "addressLine2": "Floor 3",
  "ward": "Ben Thanh",
  "district": "District 1",
  "city": "Ho Chi Minh",
  "postalCode": "700000",
  "country": "Vietnam"
}
```
- **Output**:
  - `201 Created`: tạo chi nhánh thành công
  - `400 Bad Request`: thiếu/sai dữ liệu đầu vào
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (201)**:
```json
{
  "success": true,
  "message": "Branch created successfully.",
  "data": {
    "id": 1,
    "name": "Branch HCM 01",
    "description": "Main campus in Ho Chi Minh city",
    "addressLine1": "12 Nguyen Trai",
    "addressLine2": "Floor 3",
    "ward": "Ben Thanh",
    "district": "District 1",
    "city": "Ho Chi Minh",
    "postalCode": "700000",
    "country": "Vietnam",
    "imageUrl": null
  }
}
```

## 7) Get Branch List
- **Method/Path**: `GET /branches`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input**: không có body
- **Output**:
  - `200 OK`: lấy danh sách chi nhánh thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branches fetched successfully.",
  "data": [
    {
      "id": 1,
      "name": "Branch HCM 01",
      "description": "Main campus in Ho Chi Minh city",
      "addressLine1": "12 Nguyen Trai",
      "addressLine2": "Floor 3",
      "ward": "Ben Thanh",
      "district": "District 1",
      "city": "Ho Chi Minh",
      "postalCode": "700000",
      "country": "Vietnam",
      "imageUrl": "http://localhost:9000/app-bucket/branches/1/4ab21f...ce9d.png"
    }
  ]
}
```

## 8) Get Branch By Id
- **Method/Path**: `GET /branches/{id}`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input**: path param `id` (`long`)
- **Output**:
  - `200 OK`: lấy chi nhánh thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `404 Not Found`: không tìm thấy chi nhánh
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch fetched successfully.",
  "data": {
    "id": 1,
    "name": "Branch HCM 01",
    "description": "Main campus in Ho Chi Minh city",
    "addressLine1": "12 Nguyen Trai",
    "addressLine2": "Floor 3",
    "ward": "Ben Thanh",
    "district": "District 1",
    "city": "Ho Chi Minh",
    "postalCode": "700000",
    "country": "Vietnam",
    "imageUrl": "http://localhost:9000/app-bucket/branches/1/4ab21f...ce9d.png"
  }
}
```

## 9) Update Branch
- **Method/Path**: `PUT /branches/{id}`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input**:
  - path param `id` (`long`)
  - JSON body cùng shape với create branch
- **Output**:
  - `200 OK`: cập nhật chi nhánh thành công
  - `400 Bad Request`: thiếu/sai dữ liệu đầu vào
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `404 Not Found`: không tìm thấy chi nhánh
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch updated successfully.",
  "data": {
    "id": 1,
    "name": "Branch HCM Central",
    "description": "Updated branch description",
    "addressLine1": "100 Le Loi",
    "addressLine2": null,
    "ward": "Ben Nghe",
    "district": "District 1",
    "city": "Ho Chi Minh",
    "postalCode": "700000",
    "country": "Vietnam",
    "imageUrl": "http://localhost:9000/app-bucket/branches/1/4ab21f...ce9d.png"
  }
}
```

## 10) Delete Branch (Soft Delete)
- **Method/Path**: `DELETE /branches/{id}`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input**: path param `id` (`long`)
- **Output**:
  - `200 OK`: xóa mềm chi nhánh thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `404 Not Found`: không tìm thấy chi nhánh
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch deleted successfully.",
  "data": true
}
```

## 11) Upload Branch Image
- **Method/Path**: `POST /branches/{id}/image`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input (`multipart/form-data`)**:
  - path param `id` (`long`)
  - `image` (file) - bắt buộc, hỗ trợ `image/jpeg`, `image/png`, `image/webp`
- **Output**:
  - `200 OK`: upload ảnh chi nhánh thành công
  - `400 Bad Request`: thiếu file / sai định dạng / quá dung lượng
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `404 Not Found`: không tìm thấy chi nhánh
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch image uploaded successfully.",
  "data": {
    "id": 35,
    "objectKey": "branches/1/4ab21f...ce9d.png",
    "url": "http://localhost:9000/app-bucket/branches/1/4ab21f...ce9d.png",
    "fileName": "branch-image.png",
    "contentType": "image/png",
    "size": 62811,
    "createdAt": "2026-04-23T14:00:00Z"
  }
}
```

## 12) Update Branch Image
- **Method/Path**: `PUT /branches/{id}/image`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input (`multipart/form-data`)**:
  - path param `id` (`long`)
  - `image` (file) - bắt buộc
- **Output**:
  - `200 OK`: cập nhật ảnh chi nhánh thành công
  - `400 Bad Request`: thiếu file / sai định dạng / quá dung lượng
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `404 Not Found`: không tìm thấy chi nhánh
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch image updated successfully.",
  "data": {
    "id": 36,
    "objectKey": "branches/1/6f55aa...1290.webp",
    "url": "http://localhost:9000/app-bucket/branches/1/6f55aa...1290.webp",
    "fileName": "branch-image.webp",
    "contentType": "image/webp",
    "size": 51244,
    "createdAt": "2026-04-23T14:10:00Z"
  }
}
```

## 13) Create Branch User
- **Method/Path**: `POST /branch-users`
- **Auth**: `Bearer JWT` + role `admin` (`roleId = 1`)
- **Input (JSON body)**:
```json
{
  "firstname": "Jane",
  "lastname": "Smith",
  "email": "jane.smith@example.com",
  "phonenumber": "0908889999",
  "dateOfBirth": "1999-07-10",
  "branchId": 1,
  "roleId": 4
}
```
- **Output**:
  - `201 Created`: tạo branch user thành công
  - `400 Bad Request`: thiếu/sai dữ liệu đầu vào
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `403 Forbidden`: không có quyền admin
  - `409 Conflict`: email đã tồn tại hoặc user đã thuộc branch
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (201)**:
```json
{
  "success": true,
  "message": "Branch user created successfully.",
  "data": {
    "id": 25,
    "firstname": "Jane",
    "lastname": "Smith",
    "email": "jane.smith@example.com",
    "phonenumber": "0908889999",
    "dateOfBirth": "1999-07-10",
    "branchId": 1,
    "status": 1,
    "avatarUrl": null
  }
}
```

## 14) Get Branch User List (Paginated)
- **Method/Path**: `GET /branch-users`
- **Auth**: `Bearer JWT`
- **Input (query)**:
  - `branchId` (`ulong`, optional)
  - `page` (`int`, optional, default theo `PaginationQueryDto`)
  - `limit` (`int`, optional, default theo `PaginationQueryDto`)
- **Output**:
  - `200 OK`: lấy danh sách branch user thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch users fetched successfully.",
  "data": {
    "items": [
      {
        "id": 25,
        "firstname": "Jane",
        "lastname": "Smith",
        "email": "jane.smith@example.com",
        "phonenumber": "0908889999",
        "dateOfBirth": "1999-07-10",
        "branchId": 1,
        "status": 1,
        "avatarUrl": "http://localhost:9000/app-bucket/avatars/25/8beaa9...e2c1.png"
      }
    ],
    "totalRecords": 1,
    "currentPage": 1,
    "limit": 20,
    "offset": 0
  }
}
```

## 15) Get Branch User By Id
- **Method/Path**: `GET /branch-users/{id}`
- **Auth**: `Bearer JWT`
- **Input**: path param `id` (`long`)
- **Output**:
  - `200 OK`: lấy branch user thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy branch user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch user fetched successfully.",
  "data": {
    "id": 25,
    "firstname": "Jane",
    "lastname": "Smith",
    "email": "jane.smith@example.com",
    "phonenumber": "0908889999",
    "dateOfBirth": "1999-07-10",
    "branchId": 1,
    "status": 1,
    "avatarUrl": "http://localhost:9000/app-bucket/avatars/25/8beaa9...e2c1.png"
  }
}
```

## 16) Update Branch User
- **Method/Path**: `PUT /branch-users/{id}`
- **Auth**: `Bearer JWT`
- **Input**:
  - path param `id` (`long`)
  - JSON body:
```json
{
  "status": 2
}
```
- **Output**:
  - `200 OK`: cập nhật branch user thành công
  - `400 Bad Request`: thiếu/sai dữ liệu đầu vào
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy branch user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch user updated successfully.",
  "data": {
    "id": 25,
    "firstname": "Jane",
    "lastname": "Smith",
    "email": "jane.smith@example.com",
    "phonenumber": "0908889999",
    "dateOfBirth": "1999-07-10",
    "branchId": 1,
    "status": 2,
    "avatarUrl": "http://localhost:9000/app-bucket/avatars/25/8beaa9...e2c1.png"
  }
}
```

## 17) Delete Branch User (Soft Delete)
- **Method/Path**: `DELETE /branch-users/{id}`
- **Auth**: `Bearer JWT`
- **Input**: path param `id` (`long`)
- **Output**:
  - `200 OK`: xóa mềm branch user thành công
  - `401 Unauthorized`: thiếu/sai/hết hạn token
  - `404 Not Found`: không tìm thấy branch user
  - `500 Internal Server Error`: lỗi hệ thống
- **Response mẫu (200)**:
```json
{
  "success": true,
  "message": "Branch user deleted successfully.",
  "data": true
}
```

## Response format chung
- **Success**:
```json
{
  "success": true,
  "message": "Success",
  "data": {}
}
```
- **Error**:
```json
{
  "success": false,
  "message": "Error message",
  "data": null
}
```
