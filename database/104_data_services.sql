-- table: public.data_services

-- drop table public.data_services;

create table public.data_services
(
    id bigint not null default snow_flake_id(),
    data_source_id bigint not null,
    schema character varying(16) collate pg_catalog."default",
    table_name character varying(64) collate pg_catalog."default" not null,
    fields jsonb,
    primary_key_column character varying(256) collate pg_catalog."default" not null,
    display_column character varying(256) collate pg_catalog."default" not null,
    geometry_column character varying(256) collate pg_catalog."default",
    preset_criteria character varying(128) collate pg_catalog."default",
    default_order character varying(128) collate pg_catalog."default",
    support_mvt boolean,
    mvt_min_zoom integer,
    mvt_max_zoom integer,
    mvt_cache_duration integer,
    constraint pk_data_services primary key (id),
    constraint fk_data_services_data_source_id foreign key (data_source_id)
        references public.data_sources (id) match simple
        on update no action
        on delete no action
        not valid
)
with (
    oids = false
)
tablespace pg_default;

alter table public.data_services
    owner to postgres;

comment on table public.data_services
    is '数据服务';

comment on column public.data_services.id
    is '数据服务id';

comment on column public.data_services.data_source_id
    is '数据库源id';

comment on column public.data_services.schema
    is '数据表/视图架构';

comment on column public.data_services.table_name
    is '数据表/视图名称';

comment on column public.data_services.primary_key_column
    is '主键列名称';

comment on column public.data_services.display_column
    is '显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。';

comment on column public.data_services.geometry_column
    is '空间列';

comment on column public.data_services.preset_criteria
    is '预置过滤条件';

comment on column public.data_services.default_order
    is '默认排序';

-- index: fki_fk_datasources_id

-- drop index public.fki_fk_datasources_id;

create index fki_fk_data_source_id
    on public.data_services using btree
    (data_source_id asc nulls last)
    tablespace pg_default;

comment on column public.data_services.fields
    is '数据服务允许的的字段列表';

comment on column public.data_services.support_mvt
    is '是否支持矢量切片格式';

comment on column public.data_services.mvt_min_zoom
    is '矢量切片最小级别';

comment on column public.data_services.mvt_max_zoom
    is '矢量切片最大级别';

comment on column public.data_services.mvt_cache_duration
    is '矢量切片缓存时间(秒)';
