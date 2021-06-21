-- table: public.server_folders

-- drop table public.server_folders;

create table if not exists public.server_folders
(
    id bigint not null default snow_flake_id(),
    alias_name character varying(32) collate pg_catalog."default" not null,
    root_folder character varying(128) collate pg_catalog."default" not null,
    readonly boolean not null default true,
    roles character varying(64) collate pg_catalog."default",
    constraint pk_server_folders primary key (id)
)

tablespace pg_default;

alter table public.server_folders
    owner to postgres;

comment on table public.server_folders
    is '服务器目录';

comment on column public.server_folders.id
    is '服务器目录id';

comment on column public.server_folders.alias_name
    is '目录别名';

comment on column public.server_folders.root_folder
    is '根路径';

comment on column public.server_folders.readonly
    is '是否只读';

comment on column public.server_folders.roles
    is '可访问此目录的角色列表';
