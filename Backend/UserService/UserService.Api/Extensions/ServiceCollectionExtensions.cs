namespace UserService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddHelpers(this IServiceCollection services)
    {
        services.AddTransient<IJwtHelper, JwtHelper>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient<IPermissionRepository, PermissionRepository>()
            .AddTransient<IRoleRepository, RoleRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<ISessionRepository, SessionRepository>();
    }
    
    public static void ConfigureJwtBearer(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                var signingKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);

                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });

        services.AddAuthorization(opts =>
        {
            var permissionsList = Permissions.GetRegisteredPermissions();
            foreach (var permission in permissionsList)
                opts.AddPolicy(permission, policy => 
                    policy.RequireClaim(ApplicationClaimTypes.Permission, permission));
        });
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => 
            {
                if (type.DeclaringType != null)
                {
                    return $"{type.DeclaringType.Name}_{type.Name}";
                }
                return type.Name;
            });

            
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

}