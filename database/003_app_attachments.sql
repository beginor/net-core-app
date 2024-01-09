-- table: public.app_attachments

-- drop table public.app_attachments;

create table public.app_attachments
(
    id bigint not null default snow_flake_id(),
    content_type character varying(64) collate pg_catalog."default" not null,
    file_name character varying(256) collate pg_catalog."default" not null,
    length bigint not null default 0,
    content bytea,
    business_id bigint not null,
    file_path character varying(512) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null default now(),
    creator_id character varying(32) collate pg_catalog."default" not null,
    constraint app_attachments_pkey primary key (id),
    constraint fk_app_attachments_owner foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade
)
with (
    oids = false
)
tablespace pg_default;

alter table public.app_attachments
    owner to postgres;
comment on table public.app_attachments
    is '附件表';

comment on column public.app_attachments.id
    is '附件id';

comment on column public.app_attachments.content_type
    is '内容类型（http content type）';

comment on column public.app_attachments.content
    is '附件内容';

comment on column public.app_attachments.created_at
    is '创建时间';

comment on column public.app_attachments.creator_id
    is '创建附件的用户id';

comment on column public.app_attachments.file_name
    is '文件名';

comment on column public.app_attachments.length
    is '附件大小';

comment on column public.app_attachments.business_id
    is '附件所属的业务id，可以是任意表的id，如果业务表有附件， 则需要根据业务表记录的id，删除对应的附件。';

comment on column public.app_attachments.file_path is '文件路径';
