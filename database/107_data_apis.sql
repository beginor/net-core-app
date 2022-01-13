-- table: public.data_apis

-- drop table if exists public.data_apis;

create table if not exists public.data_apis
(
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    description character varying(64) collate pg_catalog."default",
    data_source_id bigint not null,
    write_data boolean not null,
    statement xml not null,
    parameters json not null,
    columns json,
    id_column character varying(256),
    geometry_column character varying(256),
    roles character varying(32)[] collate pg_catalog."default",
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_data_apis primary key (id),
    constraint fk_data_apis_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_data_apis_data_sources foreign key (data_source_id)
        references public.data_sources (id) match simple
        on update cascade
        on delete cascade
        not valid,
    constraint fk_data_apis_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade
)

tablespace pg_default;

alter table if exists public.data_apis
    owner to postgres;

comment on table public.data_apis
    is '数据api';

comment on column public.data_apis.id
    is '数据api id';

comment on column public.data_apis.name
    is '数据api名称';

comment on column public.data_apis.description
    is '数据api描述';

comment on column public.data_apis.data_source_id
    is '数据源id';

comment on column public.data_apis.write_data
    is '是否向数据源写入数据';

comment on column public.data_apis.statement
    is '数据api调用的 xml + sql 命令';

comment on column public.data_apis.parameters
    is '参数定义';

comment on column public.data_apis.columns
    is 'api 输出列的元数据';

comment on column public.data_apis.id_column
    is '输出字段中的标识列';

comment on column public.data_apis.geometry_column
    is '输出字段中的空间列';

comment on column public.data_apis.roles
    is '允许访问的角色';

comment on column public.data_apis.creator_id
    is '创建者id';

comment on column public.data_apis.created_at
    is '创建时间';

comment on column public.data_apis.updater_id
    is '更新者id';

comment on column public.data_apis.updated_at
    is '更新时间';

comment on column public.data_apis.is_deleted
    is '是否删除';
-- index: fki_fk_data_apis_data_sources

-- drop index if exists public.fki_fk_data_apis_data_sources;

create index if not exists fki_fk_data_apis_data_sources
    on public.data_apis using btree
    (data_source_id asc nulls last)
    tablespace pg_default;
