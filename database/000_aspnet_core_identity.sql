-- SEQUENCE: public.snow_flake_id_seq

-- DROP SEQUENCE IF EXISTS public.snow_flake_id_seq;

CREATE SEQUENCE IF NOT EXISTS public.snow_flake_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.snow_flake_id_seq
    OWNER TO postgres;

-- FUNCTION: public.snow_flake_id()

-- DROP FUNCTION IF EXISTS public.snow_flake_id();

CREATE OR REPLACE FUNCTION public.snow_flake_id(
	)
    RETURNS bigint
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

select (extract(epoch from current_timestamp) * 1000)::bigint * 1000000
  + 2 * 10000
  + nextval('public.snow_flake_id_seq') % 1000
  as snow_flake_id

$BODY$;

ALTER FUNCTION public.snow_flake_id()
    OWNER TO postgres;

COMMENT ON FUNCTION public.snow_flake_id()
    IS '雪花ID';

-- Table: public.aspnet_roles

-- DROP TABLE IF EXISTS public.aspnet_roles;

CREATE TABLE IF NOT EXISTS public.aspnet_roles
(
    id character varying(32) COLLATE pg_catalog."default" NOT NULL DEFAULT (snow_flake_id())::character varying,
    name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    normalized_name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    concurrency_stamp character varying(36) COLLATE pg_catalog."default",
    CONSTRAINT pk_aspnet_roles PRIMARY KEY (id),
    CONSTRAINT u_aspnet_roles_name UNIQUE (name),
    CONSTRAINT u_aspnet_roles_normalized_name UNIQUE (normalized_name)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_roles
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_roles
    IS '角色基础信息';

COMMENT ON COLUMN public.aspnet_roles.id
    IS '角色ID';

COMMENT ON COLUMN public.aspnet_roles.name
    IS '角色名称';

COMMENT ON COLUMN public.aspnet_roles.normalized_name
    IS '规范化的角色名称';

COMMENT ON COLUMN public.aspnet_roles.concurrency_stamp
    IS '并发印记';
-- Index: ix_aspnet_roles_name

-- DROP INDEX IF EXISTS public.ix_aspnet_roles_name;

CREATE INDEX IF NOT EXISTS ix_aspnet_roles_name
    ON public.aspnet_roles USING btree
    (normalized_name COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_role_claims

-- DROP TABLE IF EXISTS public.aspnet_role_claims;

CREATE TABLE IF NOT EXISTS public.aspnet_role_claims
(
    id integer NOT NULL DEFAULT nextval('snow_flake_id_seq'::regclass),
    role_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    claim_type character varying(1024) COLLATE pg_catalog."default" NOT NULL,
    claim_value character varying(1024) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_aspnet_role_claims PRIMARY KEY (id),
    CONSTRAINT fk_aspnet_roles_id FOREIGN KEY (role_id)
        REFERENCES public.aspnet_roles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_role_claims
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_role_claims
    IS '角色属性';

COMMENT ON COLUMN public.aspnet_role_claims.id
    IS '属性ID';

COMMENT ON COLUMN public.aspnet_role_claims.role_id
    IS '角色ID';

COMMENT ON COLUMN public.aspnet_role_claims.claim_type
    IS '属性名称';

COMMENT ON COLUMN public.aspnet_role_claims.claim_value
    IS '属性值';
-- Index: ix_aspnet_role_claims_role_id

-- DROP INDEX IF EXISTS public.ix_aspnet_role_claims_role_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_role_claims_role_id
    ON public.aspnet_role_claims USING btree
    (role_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_users

-- DROP TABLE IF EXISTS public.aspnet_users;

CREATE TABLE IF NOT EXISTS public.aspnet_users
(
    id character varying(32) COLLATE pg_catalog."default" NOT NULL DEFAULT (snow_flake_id())::character varying,
    user_name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    normalized_user_name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    email character varying(256) COLLATE pg_catalog."default" NOT NULL,
    normalized_email character varying(256) COLLATE pg_catalog."default" NOT NULL,
    email_confirmed boolean NOT NULL,
    phone_number character varying(32) COLLATE pg_catalog."default",
    phone_number_confirmed boolean NOT NULL,
    lockout_enabled boolean NOT NULL,
    lockout_end_unix_time_seconds bigint,
    password_hash character varying(256) COLLATE pg_catalog."default",
    access_failed_count integer NOT NULL,
    security_stamp character varying(256) COLLATE pg_catalog."default",
    two_factor_enabled boolean NOT NULL,
    concurrency_stamp character varying(36) COLLATE pg_catalog."default",
    CONSTRAINT pk_aspnet_users PRIMARY KEY (id),
    CONSTRAINT u_aspnet_users_normalized_user_name UNIQUE (normalized_user_name),
    CONSTRAINT u_aspnet_users_username UNIQUE (user_name)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_users
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_users
    IS '用户基础信息';

COMMENT ON COLUMN public.aspnet_users.id
    IS '用户ID';

COMMENT ON COLUMN public.aspnet_users.user_name
    IS '用户名';

COMMENT ON COLUMN public.aspnet_users.normalized_user_name
    IS '规范化的用户名';

COMMENT ON COLUMN public.aspnet_users.email
    IS '邮箱';

COMMENT ON COLUMN public.aspnet_users.normalized_email
    IS '规范化的邮箱';

COMMENT ON COLUMN public.aspnet_users.email_confirmed
    IS '邮箱是否确认';

COMMENT ON COLUMN public.aspnet_users.phone_number
    IS '电话';

COMMENT ON COLUMN public.aspnet_users.phone_number_confirmed
    IS '电话是否确认';

COMMENT ON COLUMN public.aspnet_users.lockout_enabled
    IS '是否允许锁定';

COMMENT ON COLUMN public.aspnet_users.lockout_end_unix_time_seconds
    IS '锁定结束时间';

COMMENT ON COLUMN public.aspnet_users.password_hash
    IS '密码哈希';

COMMENT ON COLUMN public.aspnet_users.access_failed_count
    IS '登录失败次数';

COMMENT ON COLUMN public.aspnet_users.security_stamp
    IS '安全印记';

COMMENT ON COLUMN public.aspnet_users.two_factor_enabled
    IS '是否启用两步认证';

COMMENT ON COLUMN public.aspnet_users.concurrency_stamp
    IS '并发印记';
-- Index: ix_aspnet_users_email

-- DROP INDEX IF EXISTS public.ix_aspnet_users_email;

CREATE INDEX IF NOT EXISTS ix_aspnet_users_email
    ON public.aspnet_users USING btree
    (normalized_email COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: ix_aspnet_users_user_name

-- DROP INDEX IF EXISTS public.ix_aspnet_users_user_name;

CREATE UNIQUE INDEX IF NOT EXISTS ix_aspnet_users_user_name
    ON public.aspnet_users USING btree
    (normalized_user_name COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_user_claims

-- DROP TABLE IF EXISTS public.aspnet_user_claims;

CREATE TABLE IF NOT EXISTS public.aspnet_user_claims
(
    id integer NOT NULL DEFAULT nextval('snow_flake_id_seq'::regclass),
    user_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    claim_type character varying(1024) COLLATE pg_catalog."default" NOT NULL,
    claim_value character varying(1024) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_aspnet_user_claims PRIMARY KEY (id),
    CONSTRAINT fk_aspnet_users_id FOREIGN KEY (user_id)
        REFERENCES public.aspnet_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_user_claims
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_user_claims
    IS '用户属性';

COMMENT ON COLUMN public.aspnet_user_claims.id
    IS '用户属性ID';

COMMENT ON COLUMN public.aspnet_user_claims.user_id
    IS '用户ID';

COMMENT ON COLUMN public.aspnet_user_claims.claim_type
    IS '属性名称';

COMMENT ON COLUMN public.aspnet_user_claims.claim_value
    IS '属性值';
-- Index: ix_aspnet_user_claims_user_id

-- DROP INDEX IF EXISTS public.ix_aspnet_user_claims_user_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_user_claims_user_id
    ON public.aspnet_user_claims USING btree
    (user_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_user_logins

-- DROP TABLE IF EXISTS public.aspnet_user_logins;

CREATE TABLE IF NOT EXISTS public.aspnet_user_logins
(
    login_provider character varying(32) COLLATE pg_catalog."default" NOT NULL,
    provider_key character varying(1024) COLLATE pg_catalog."default" NOT NULL,
    provider_display_name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    user_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_aspnet_user_logins PRIMARY KEY (login_provider, provider_key),
    CONSTRAINT fk_aspnet_user_logins_user_id FOREIGN KEY (user_id)
        REFERENCES public.aspnet_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_user_logins
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_user_logins
    IS '用户登录方式';

COMMENT ON COLUMN public.aspnet_user_logins.login_provider
    IS '登录提供方';

COMMENT ON COLUMN public.aspnet_user_logins.provider_key
    IS '提供方特征值';

COMMENT ON COLUMN public.aspnet_user_logins.provider_display_name
    IS '提供方显示名称';

COMMENT ON COLUMN public.aspnet_user_logins.user_id
    IS '用户ID';
-- Index: ix_aspnet_user_logins_user_id

-- DROP INDEX IF EXISTS public.ix_aspnet_user_logins_user_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_user_logins_user_id
    ON public.aspnet_user_logins USING btree
    (user_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_user_tokens

-- DROP TABLE IF EXISTS public.aspnet_user_tokens;

CREATE TABLE IF NOT EXISTS public.aspnet_user_tokens
(
    user_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    login_provider character varying(32) COLLATE pg_catalog."default" NOT NULL,
    name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    value character varying(256) COLLATE pg_catalog."default",
    CONSTRAINT pk_aspnet_user_tokens PRIMARY KEY (user_id, login_provider, name),
    CONSTRAINT fk_aspnet_users_id FOREIGN KEY (user_id)
        REFERENCES public.aspnet_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_user_tokens
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_user_tokens
    IS '用户令牌';

COMMENT ON COLUMN public.aspnet_user_tokens.user_id
    IS '用户ID';

COMMENT ON COLUMN public.aspnet_user_tokens.login_provider
    IS '登录提供方';

COMMENT ON COLUMN public.aspnet_user_tokens.name
    IS '令牌名称';

COMMENT ON COLUMN public.aspnet_user_tokens.value
    IS '令牌值';
-- Index: ix_aspnet_user_tokens_user_id

-- DROP INDEX IF EXISTS public.ix_aspnet_user_tokens_user_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_user_tokens_user_id
    ON public.aspnet_user_tokens USING btree
    (user_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.aspnet_user_roles

-- DROP TABLE IF EXISTS public.aspnet_user_roles;

CREATE TABLE IF NOT EXISTS public.aspnet_user_roles
(
    user_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    role_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_aspnet_user_roles PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_aspnet_roles_id FOREIGN KEY (role_id)
        REFERENCES public.aspnet_roles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_aspnet_users_id FOREIGN KEY (user_id)
        REFERENCES public.aspnet_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.aspnet_user_roles
    OWNER to postgres;

COMMENT ON TABLE public.aspnet_user_roles
    IS '用户角色关联';

COMMENT ON COLUMN public.aspnet_user_roles.user_id
    IS '用户ID';

COMMENT ON COLUMN public.aspnet_user_roles.role_id
    IS '角色ID';
-- Index: ix_aspnet_user_roles_role_id

-- DROP INDEX IF EXISTS public.ix_aspnet_user_roles_role_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_user_roles_role_id
    ON public.aspnet_user_roles USING btree
    (role_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: ix_aspnet_user_roles_user_id

-- DROP INDEX IF EXISTS public.ix_aspnet_user_roles_user_id;

CREATE INDEX IF NOT EXISTS ix_aspnet_user_roles_user_id
    ON public.aspnet_user_roles USING btree
    (user_id COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
