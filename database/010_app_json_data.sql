-- table: public.app_json_data

-- drop table if exists public.app_json_data;

create table if not exists public.app_json_data
(
    id bigint not null default snow_flake_id(),
    value jsonb not null,
    business_id bigint not null default 0,
    name character varying(64) collate pg_catalog."default" not null default ''::character varying,
    updated_at timestamp without time zone not null default now(),
    constraint pk_app_json_data primary key (id)
)

tablespace pg_default;

alter table if exists public.app_json_data
    owner to postgres;

comment on table public.app_json_data
    is 'json 数据';

comment on column public.app_json_data.id
    is 'json 数据id';

comment on column public.app_json_data.value
    is 'json 值';

comment on column public.app_json_data.business_id
    is '业务id';

comment on column public.app_json_data.name
    is '名称';

comment on column public.app_json_data.updated_at
    is '更新时间';
