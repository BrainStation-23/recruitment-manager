﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using RecruitmentManagementSystem.Core.Mappings;
using RecruitmentManagementSystem.Model;

namespace RecruitmentManagementSystem.Core.Models.Account
{
    public class ApplicationUserModel : IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IList<string> Roles { get; set; }

        [Display(Name = "Avatar")]
        public File Avatar { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Model.User, ApplicationUserModel>()
                .ForMember(dest => dest.Roles, (IMemberConfigurationExpression<Model.User> opt) => opt.Ignore());
        }
    }
}