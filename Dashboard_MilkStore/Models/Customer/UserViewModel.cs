using System;
using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Customer
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
    }

    public class UserQueryViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public string StatusId { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortAscending { get; set; } = false;
    }

    public class UserMetadata
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public int FirstItemOnPage { get; set; }
    }

    public class UserResponse
    {
        public UserMetadata Metadata { get; set; } = new();
        public List<UserViewModel> Items { get; set; } = new();
    }
}
