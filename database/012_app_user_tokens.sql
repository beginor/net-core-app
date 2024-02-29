-- Table: public.app_user_tokens

-- DROP TABLE IF EXISTS public.app_user_tokens;

CREATE TABLE IF NOT EXISTS public.app_user_tokens
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    user_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    name character varying(16) COLLATE pg_catalog."default" NOT NULL,
    value character varying(32) COLLATE pg_catalog."default" NOT NULL,
    privileges character varying(64)[] COLLATE pg_catalog."default",
    urls character varying(64)[] COLLATE pg_catalog."default",
    update_time timestamp without time zone NOT NULL,
    expires_at timestamp without time zone,
    roles character varying(64)[] COLLATE pg_catalog."default",
    CONSTRAINT pk_app_user_tokens PRIMARY KEY (id),
    CONSTRAINT uk_app_user_tokens_user_id_name UNIQUE (user_id, name),
    CONSTRAINT fk_app_user_tokens_user_id FOREIGN KEY (user_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_user_tokens
    OWNER to postgres;

COMMENT ON TABLE public.app_user_tokens
    IS '用户凭证';

COMMENT ON COLUMN public.app_user_tokens.id
    IS '凭证ID';

COMMENT ON COLUMN public.app_user_tokens.user_id
    IS '用户ID';

COMMENT ON COLUMN public.app_user_tokens.name
    IS '凭证名称';

COMMENT ON COLUMN public.app_user_tokens.value
    IS '凭证值';

COMMENT ON COLUMN public.app_user_tokens.privileges
    IS '凭证权限';

COMMENT ON COLUMN public.app_user_tokens.urls
    IS '允许的 url 地址';

COMMENT ON COLUMN public.app_user_tokens.update_time
    IS '更新时间';

COMMENT ON COLUMN public.app_user_tokens.expires_at
    IS '过期时间';

COMMENT ON COLUMN public.app_user_tokens.roles
    IS '凭证代表的角色';
