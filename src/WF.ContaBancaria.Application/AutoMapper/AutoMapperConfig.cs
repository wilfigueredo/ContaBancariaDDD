﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF.ContaBancaria.Application.AutoMapper
{
    public class AutoMapperConfig
    {
        public static void RegisterMapings()
        {
            Mapper.Initialize
                (m => { m.AddProfile<DomainToViewModelMappingProfile>(); });
        }
    }
}
