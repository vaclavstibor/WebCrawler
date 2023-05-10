using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace WebCrawler.BusinessLayer.Services.Base
{
    public abstract class ServiceBase<TEntity, TDto>
    {
        protected readonly IMapper Mapper;

        protected ServiceBase(IMapper mapper)
        {
            Mapper = mapper;
        }

        protected TDto EntityToDto(TEntity entity)
        {
            return Mapper.Map<TDto>(entity);
        }
        protected IQueryable<TDto> ProjectTo(IQueryable<TEntity> queryable)
        {
            return queryable.ProjectTo<TDto>(Mapper.ConfigurationProvider);
        }

        protected List<TDto> EntitiesToDTOes(List<TEntity> list)
        { 
            var dtoes = new List<TDto>();
            foreach (var item in list)
            {
                dtoes.Add(Mapper.Map<TDto>(item));
            }
            return dtoes;
        }

        protected List<TEntity> DTOesToEntities(List<TDto> list)
        {
            var entities = new List<TEntity>();
            foreach (var item in list)
            {
                entities.Add(Mapper.Map<TEntity>(item));
            }
            return entities;
        }
    }
    
}
