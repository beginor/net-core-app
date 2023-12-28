-- table: public.app_users

-- drop table if exists public.app_users;

create table if not exists public.app_users
(
    id character varying(32) collate pg_catalog."default" not null,
    create_time timestamp without time zone not null default now(),
    last_login timestamp without time zone,
    login_count integer default 0,
    organize_unit_id bigint not null default 0,
    display_name character varying(64) collate pg_catalog."default",
    constraint pk_app_users primary key (id),
    constraint fk_app_users_organize_unit foreign key (organize_unit_id)
        references public.app_organize_units (id) match simple
        on update no action
        on delete no action
        not valid,
    constraint fk_aspnet_users_id foreign key (id)
        references public.aspnet_users (id) match simple
        on update cascade
        on delete cascade
)

tablespace pg_default;

alter table if exists public.app_users
    owner to postgres;

comment on table public.app_users
    is 'application users table.';

comment on column public.app_users.create_time
    is 'create time';

comment on column public.app_users.last_login
    is 'last login time';

comment on column public.app_users.login_count
    is 'login count';

comment on column public.app_users.organize_unit_id
    is 'organize unit id';

comment on column public.app_users.display_name
    is 'display name';
