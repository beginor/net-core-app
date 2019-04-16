-- Table: public.attachments

-- DROP TABLE public.attachments;

CREATE TABLE public.attachments
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    content_type character varying(64) COLLATE pg_catalog."default" NOT NULL,
    content bytea NOT NULL,
    create_time timestamp without time zone NOT NULL DEFAULT now(),
    user_id character varying(32) COLLATE pg_catalog."default",
    CONSTRAINT attachments_pkey PRIMARY KEY (id),
    CONSTRAINT fk_attachments_user FOREIGN KEY (user_id)
        REFERENCES public.application_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.attachments
    OWNER to postgres;
COMMENT ON TABLE public.attachments
    IS '附件表';

COMMENT ON COLUMN public.attachments.id
    IS '附件ID';

COMMENT ON COLUMN public.attachments.content_type
    IS '内容类型（HTTP Content Type）';

COMMENT ON COLUMN public.attachments.content
    IS '附件内容';

COMMENT ON COLUMN public.attachments.create_time
    IS '创建时间';

COMMENT ON COLUMN public.attachments.user_id
    IS '创建附件的用户ID';
