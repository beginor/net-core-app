-- table: public.data_apis

-- drop table if exists public.data_apis;

create table if not exists public.data_apis
(
    id bigint not null default snow_flake_id(),
    data_source_id bigint not null,
    write_data boolean not null,
    statement xml not null,
    parameters json not null,
    columns json,
    id_column character varying(256) collate pg_catalog."default",
    geometry_column character varying(256) collate pg_catalog."default",
    constraint pk_data_apis primary key (id),
    constraint fk_data_apis_data_sources foreign key (data_source_id)
        references public.data_sources (id) match simple
        on update cascade
        on delete cascade
        not valid
)

tablespace pg_default;

alter table if exists public.data_apis
    owner to postgres;

comment on table public.data_apis
    is '数据api';

comment on column public.data_apis.id
    is '数据api id';

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
    is '输出字段中的标识列名称';

comment on column public.data_apis.geometry_column
    is '输出字段中的空间列';
-- index: fki_fk_data_apis_data_sources

-- drop index if exists public.fki_fk_data_apis_data_sources;

create index if not exists fki_fk_data_apis_data_sources
    on public.data_apis using btree
    (data_source_id asc nulls last)
    tablespace pg_default;
