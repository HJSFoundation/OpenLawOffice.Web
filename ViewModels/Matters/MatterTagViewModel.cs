﻿namespace OpenLawOffice.WebClient.ViewModels.Matters
{
    using System;
    using AutoMapper;
    using OpenLawOffice.Common.Models;

    [MapMe]
    public class MatterTagViewModel : Tagging.TagBaseViewModel
    {
        public MatterViewModel Matter { get; set; }

        public void BuildMappings()
        {
            Mapper.CreateMap<DBOs.Matters.MatterTag, MatterTagViewModel>()
                .ForMember(dst => dst.IsStub, opt => opt.UseValue(false))
                .ForMember(dst => dst.UtcCreated, opt => opt.MapFrom(src => src.UtcCreated))
                .ForMember(dst => dst.UtcModified, opt => opt.MapFrom(src => src.UtcModified))
                .ForMember(dst => dst.UtcDisabled, opt => opt.MapFrom(src => src.UtcDisabled))
                .ForMember(dst => dst.CreatedBy, opt => opt.ResolveUsing(db =>
                {
                    return new ViewModels.Security.UserViewModel()
                    {
                        Id = db.CreatedByUserId,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.ModifiedBy, opt => opt.ResolveUsing(db =>
                {
                    return new ViewModels.Security.UserViewModel()
                    {
                        Id = db.ModifiedByUserId,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.DisabledBy, opt => opt.ResolveUsing(db =>
                {
                    if (!db.DisabledByUserId.HasValue) return null;
                    return new ViewModels.Security.UserViewModel()
                    {
                        Id = db.DisabledByUserId.Value,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Matter, opt => opt.ResolveUsing(db =>
                {
                    return new ViewModels.Matters.MatterViewModel()
                    {
                        Id = db.MatterId,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.TagCategory, opt => opt.ResolveUsing(db =>
                {
                    if (!db.TagCategoryId.HasValue || db.TagCategoryId.Value < 1)
                        return null;

                    return new ViewModels.Tagging.TagCategoryViewModel()
                    {
                        Id = db.TagCategoryId.Value,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Tag, opt => opt.MapFrom(src => src.Tag));

            Mapper.CreateMap<MatterTagViewModel, DBOs.Matters.MatterTag>()
                .ForMember(dst => dst.UtcCreated, opt => opt.MapFrom(src => src.UtcCreated))
                .ForMember(dst => dst.UtcModified, opt => opt.MapFrom(src => src.UtcModified))
                .ForMember(dst => dst.UtcDisabled, opt => opt.MapFrom(src => src.UtcDisabled))
                .ForMember(dst => dst.CreatedByUserId, opt => opt.ResolveUsing(model =>
                {
                    if (model.CreatedBy == null || !model.CreatedBy.Id.HasValue)
                        return 0;
                    return model.CreatedBy.Id.Value;
                }))
                .ForMember(dst => dst.ModifiedByUserId, opt => opt.ResolveUsing(model =>
                {
                    if (model.ModifiedBy == null || !model.ModifiedBy.Id.HasValue)
                        return 0;
                    return model.ModifiedBy.Id.Value;
                }))
                .ForMember(dst => dst.DisabledByUserId, opt => opt.ResolveUsing(model =>
                {
                    if (model.DisabledBy == null) return null;
                    return model.DisabledBy.Id;
                }))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.MatterId, opt => opt.ResolveUsing(model =>
                {
                    if (model.Matter == null || !model.Matter.Id.HasValue)
                        throw new Exception("Matter cannot be null");
                    return model.Matter.Id.Value;
                }))
                .ForMember(dst => dst.TagCategoryId, opt => opt.ResolveUsing(model =>
                {
                    if (model.TagCategory == null)
                        return null;
                    return model.TagCategory.Id;
                }))
                .ForMember(dst => dst.Tag, opt => opt.MapFrom(src => src.Tag));
        }
    }
}