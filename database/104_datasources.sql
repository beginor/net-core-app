-- table: public.datasources

-- drop table public.datasources;

create table public.datasources
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    connection_id bigint not null,
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
    constraint pk_datasources primary key (id),
    constraint fk_datasources_connection_id foreign key (connection_id)
        references public.connections (id) match simple
        on update no action
        on delete no action
        not valid
)
with (
    oids = false
)
tablespace pg_default;

alter table public.datasources
    owner to postgres;

comment on table public.datasources
    is '数据源（数据表或视图）';

comment on column public.datasources.id
    is '数据源id';

comment on column public.datasources.name
    is '数据源名称';

comment on column public.datasources.connection_id
    is '数据库连接串id';

comment on column public.datasources.schema
    is '数据表/视图架构';

comment on column public.datasources.table_name
    is '数据表/视图名称';

comment on column public.datasources.primary_key_column
    is '主键列名称';

comment on column public.datasources.display_column
    is '显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。';

comment on column public.datasources.geometry_column
    is '空间列';

comment on column public.datasources.preset_criteria
    is '预置过滤条件';

comment on column public.datasources.default_order
    is '默认排序';

comment on column public.datasources.tags
    is '标签';

comment on column public.datasources.is_deleted
    is '是否删除';
-- index: fki_fk_connections_id

-- drop index public.fki_fk_connections_id;

create index fki_fk_connections_id
    on public.datasources using btree
    (connection_id asc nulls last)
    tablespace pg_default;

comment on column public.datasources.roles
    is '允许的角色';

alter table public.datasources
    add column description character varying(256) collate pg_catalog."default";

comment on column public.datasources.description
    is '数据源描述';

alter table public.datasources
    add column fields jsonb;

comment on column public.datasources.fields
    is '数据源允许的的字段列表';
