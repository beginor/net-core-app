-- table: public.app_dictionaries

-- drop table public.app_dictionaries;

create table public.app_dictionaries
(
    id bigint not null default snow_flake_id(),
    name character varying(50) collate pg_catalog."default",
    value character varying(50) collate pg_catalog."default",
    keyword character varying(50) collate pg_catalog."default",
    order_no integer,
    description character varying(255) collate pg_catalog."default",
    flag character varying(50) collate pg_catalog."default",
    is_active boolean not null default false,
    created_at timestamp(4) without time zone default now(),
    constraint pk_app_dictionaries primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_dictionaries
    owner to postgres;
comment on table public.app_dictionaries
    is '系统字典表';

comment on column public.app_dictionaries.id
    is '主键id';

comment on column public.app_dictionaries.name
    is '字典名称';

comment on column public.app_dictionaries.value
    is '字典值';

comment on column public.app_dictionaries.keyword
    is '字典关键字';

comment on column public.app_dictionaries.order_no
    is '排序号';

comment on column public.app_dictionaries.description
    is '描述';

comment on column public.app_dictionaries.flag
    is '系统标识';

comment on column public.app_dictionaries.is_active
    is '是否启用';
