-- table: public.data_services

-- drop table public.data_services;

create table public.data_services
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    datasource_id bigint not null,
    schema character varying(16) collate pg_catalog."default",
    table_name character varying(64) collate pg_catalog."default" not null,
    primary_key_column character varying(256) collate pg_catalog."default" not null,
    display_column character varying(256) collate pg_catalog."default" not null,
    geometry_column character varying(256) collate pg_catalog."default",
    preset_criteria character varying(128) collate pg_catalog."default",
    default_order character varying(128) collate pg_catalog."default",
    tags character varying(16)[] collate pg_catalog."default",
    is_deleted boolean not null default false,
    roles character varying(64)[] collate pg_catalog."default",
    constraint pk_data_services primary key (id),
    constraint fk_data_services_datasource_id foreign key (datasource_id)
        references public.datasources (id) match simple
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

comment on column public.data_services.name
    is '数据服务名称';

comment on column public.data_services.datasource_id
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

comment on column public.data_services.tags
    is '标签';

comment on column public.data_services.is_deleted
    is '是否删除';

-- index: fki_fk_datasources_id

-- drop index public.fki_fk_datasources_id;

create index fki_fk_datasource_id
    on public.data_services using btree
    (datasource_id asc nulls last)
    tablespace pg_default;

comment on column public.data_services.roles
    is '允许的角色';

alter table public.data_services
    add column description character varying(256) collate pg_catalog."default";

comment on column public.data_services.description
    is '数据服务描述';

alter table public.data_services
    add column fields jsonb;

comment on column public.data_services.fields
    is '数据服务允许的的字段列表';
