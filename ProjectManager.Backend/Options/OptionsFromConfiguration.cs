namespace ProjectManager.Backend.Options; 

public abstract class OptionsFromConfiguration {
    public abstract string Position { get; set; }
}

public static class OptionsFromConfigurationExtensions {

    public static T AddOptionsFromConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : OptionsFromConfiguration {
        T optionsInstance = (T)Activator.CreateInstance(typeof(T));
        if (optionsInstance == null) return null;
        var position = optionsInstance.Position;

        services.Configure((Action<T>)(options => {
            configuration.Bind(position, options);
        }));

        return optionsInstance;
    }
    
}