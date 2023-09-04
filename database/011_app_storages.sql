-- table: public.app_storages

-- drop table public.app_storages;

create table if not exists public.app_storages
(
    id bigint not null default snow_flake_id(),
    alias_name character varying(32) collate pg_catalog."default" not null,
    root_folder character varying(128) collate pg_catalog."default" not null,
    readonly boolean not null default true,
    roles character varying(64)[] collate pg_catalog."default",
    constraint pk_app_storages primary key (id)
)

tablespace pg_default;

alter table public.app_storages
    owner to postgres;

comment on table public.app_storages
    is '应用存储';

comment on column public.app_storages.id
    is '存储id';

comment on column public.app_storages.alias_name
    is '存储别名';

comment on column public.app_storages.root_folder
    is '存储根路径';

comment on column public.app_storages.readonly
    is '是否只读';

comment on column public.app_storages.roles
    is '可访问此存储的角色';

INSERT INTO public.app_storages (alias_name, root_folder, readonly, roles)
VALUES ('icons', '!/web/assets/icons/', true, '{users}');
