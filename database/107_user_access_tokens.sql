-- table: public.user_access_tokens

-- drop table public.user_access_tokens;

create table if not exists public.user_access_tokens
(
    id bigint not null default snow_flake_id(),
    user_id character varying(32) collate pg_catalog."default" not null,
    name character varying(16) collate pg_catalog."default" not null,
    value character varying(32) collate pg_catalog."default" not null,
    privileges character varying(64)[] collate pg_catalog."default",
    urls character varying(64)[] collate pg_catalog."default",
    update_time timestamp without time zone not null,
    constraint pk_user_access_tokens primary key (id),
    constraint uk_user_access_tokens_user_id_name unique (user_id, name),
    constraint fk_user_access_tokens_user_id foreign key (user_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
        not valid
)
tablespace pg_default;

alter table public.user_access_tokens owner to postgres;

comment on table public.user_access_tokens
    is '用户访问凭证';

comment on column public.user_access_tokens.id
    is '凭证id';

comment on column public.user_access_tokens.name
    is '凭证名称';

comment on column public.user_access_tokens.value
    is '凭证值';

comment on column public.user_access_tokens.update_time
    is '更新时间';

comment on column public.user_access_tokens.user_id
    is '用户id';

comment on column public.user_access_tokens.urls
    is '允许的 url 地址';

comment on column public.user_access_tokens.privileges
    is '凭证权限';
