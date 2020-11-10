-- table: public.connection_strings

-- drop table public.connection_strings;

create table public.connection_strings
(
    id bigint not null default snow_flake_id(),
    name character varying(64) collate pg_catalog."default" not null,
    value character varying(512) collate pg_catalog."default" not null,
    database_type character varying(16) collate pg_catalog."default" not null,
    deleted boolean not null default false,
    constraint pk_connection_strings primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.connection_strings
    owner to postgres;

comment on table public.connection_strings
    is '数据库连接串';

comment on column public.connection_strings.id
    is '连接串id';

comment on column public.connection_strings.name
    is '连接串名称';

comment on column public.connection_strings.value
    is '连接串值';

comment on column public.connection_strings.database_type
    is '数据库类型（postgres、mssql、mysql、oracle、sqlite等）';

comment on column public.connection_strings.deleted
    is '是否已删除（软删除）';
