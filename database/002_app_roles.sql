-- table: public.app_roles

-- drop table public.app_roles;

create table public.app_roles
(
    id character varying(32) collate pg_catalog."default" not null,
    description character varying(256) collate pg_catalog."default",
    is_default boolean default false,
    is_anonymous boolean default false,
    constraint pk_app_roles primary key (id),
    constraint fk_aspnet_roles_id foreign key (id)
        references public.aspnet_roles (id) match simple
        on update cascade
        on delete cascade
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_roles
    owner to postgres;
comment on table public.app_roles
    is 'application roles table.';

comment on column public.app_roles.description
    is 'roles description';

comment on column public.app_roles.is_default
    is 'is default role';

comment on column public.app_roles.is_anonymous
    is 'is anonymous role';
