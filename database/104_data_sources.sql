-- table: public.data_sources

-- drop table public.data_sources;

create table public.data_sources
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    connection_string_id bigint not null,
    schema character varying(16) collate pg_catalog."default",
    table_name character varying(64) collate pg_catalog."default" not null,
    primary_key_column character varying(256) collate pg_catalog."default" not null,
    display_column character varying(256) collate pg_catalog."default" not null,
    geometry_column character varying(256) collate pg_catalog."default",
    preset_criteria character varying(128) collate pg_catalog."default",
    default_order character varying(128) collate pg_catalog."default",
    tags character varying(16)[] collate pg_catalog."default",
    deleted boolean not null default false,
    constraint pk_data_sources primary key (id),
    constraint fk_connection_string_id foreign key (connection_string_id)
        references public.data_sources (id) match simple
        on update cascade
        on delete cascade
)
with (
    oids = false
)
tablespace pg_default;

alter table public.data_sources
    owner to postgres;

comment on table public.data_sources
    is '数据源（数据表或视图）';

comment on column public.data_sources.id
    is '数据源id';

comment on column public.data_sources.name
    is '数据源名称';

comment on column public.data_sources.connection_string_id
    is '数据库连接串id';

comment on column public.data_sources.schema
    is '数据表/视图架构';

comment on column public.data_sources.table_name
    is '数据表/视图名称';

comment on column public.data_sources.primary_key_column
    is '主键列名称';

comment on column public.data_sources.display_column
    is '显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。';

comment on column public.data_sources.geometry_column
    is '空间列';

comment on column public.data_sources.tags
    is '标签';

comment on column public.data_sources.deleted
    is '是否删除';

comment on column public.data_sources.preset_criteria
    is '预置过滤条件';

comment on column public.data_sources.default_order
    is '默认排序';
