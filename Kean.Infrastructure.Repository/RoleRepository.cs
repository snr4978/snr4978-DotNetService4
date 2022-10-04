﻿using AutoMapper;
using Kean.Domain.Admin.Models;
using Kean.Domain.Admin.Repositories;
using Kean.Infrastructure.Database;
using Kean.Infrastructure.Database.Repository.Default;
using Kean.Infrastructure.Database.Repository.Default.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kean.Infrastructure.Repository
{
    /// <summary>
    /// 角色仓库
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly IMapper _mapper; // 模型映射
        private readonly IDefaultDb _database; // 默认数据库

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleRepository(
            IMapper mapper,
            IDefaultDb database)
        {
            _mapper = mapper;
            _database = database;
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.IsExist(int id) 方法
         */
        public async Task<bool> IsExist(int id)
        {
            return (await _database.From<T_SYS_ROLE>()
                .Where(r => r.ROLE_ID == id)
                .Single(r => new { Count = Function.Count(r.ROLE_ID) })).Count > 0;
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.IsExist(string name, int? igrone) 方法
         */
        public async Task<bool> IsExist(string name, int? igrone)
        {
            var schema = _database.From<T_SYS_ROLE>().Where(r => r.ROLE_NAME == name);
            if (igrone.HasValue)
            {
                schema = schema.Where(r => r.ROLE_ID != igrone.Value);
            }
            return (await schema.Single(r => new { Count = Function.Count(r.ROLE_ID) })).Count > 0;
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.Create(Role role) 方法
         */
        public async Task<int> Create(Role role)
        {
            var entity = _mapper.Map<T_SYS_ROLE>(role);
            entity.UPDATE_TIME = entity.CREATE_TIME = DateTime.Now;
            var id = await _database.From<T_SYS_ROLE>().Add(entity);
            return Convert.ToInt32(id);
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.Modify(Role role) 方法
         */
        public Task Modify(Role role)
        {
            var entity = _mapper.Map<T_SYS_ROLE>(role);
            entity.UPDATE_TIME = DateTime.Now;
            return _database.From<T_SYS_ROLE>().Update(entity, nameof(T_SYS_ROLE.CREATE_TIME));
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.Delete(int id) 方法
         */
        public async Task Delete(int id)
        {
            await _database.From<T_SYS_ROLE>()
                .Where(r => r.ROLE_ID == id)
                .Delete();
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.SetMenuPermission(int id, IEnumerable<int> permission) 方法
         */
        public async Task SetMenuPermission(int id, IEnumerable<int> permission)
        {
            await _database.From<T_SYS_ROLE_MENU>()
                .Where(r => r.ROLE_ID == id)
                .Delete();
            var timestamp = DateTime.Now;
            foreach (var item in permission)
            {
                await _database.From<T_SYS_ROLE_MENU>().Add(new()
                {
                    ROLE_ID = id,
                    MENU_ID = item,
                    CREATE_TIME = timestamp,
                    UPDATE_TIME = timestamp
                });
            }
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.ClearMenuPermission(int id) 方法
         */
        public async Task ClearMenuPermission(int id)
        {
            await _database.From<T_SYS_ROLE_MENU>()
                .Where(r => r.ROLE_ID == id)
                .Delete();
        }

        /*
         * 实现 Kean.Domain.Admin.Repositories.IRoleRepository.ClearUserRole(int id) 方法
         */
        public async Task ClearUserRole(int id)
        {
            await _database.From<T_SYS_USER_ROLE>()
                .Where(r => r.ROLE_ID == id)
                .Delete();
        }
    }
}
