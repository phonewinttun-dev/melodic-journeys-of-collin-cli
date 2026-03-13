window.appTheme = (() => {
  const storageKey = "collin-theme";

  const resolveMode = (preference) => {
    if (preference === "light") {
      return false;
    }

    if (preference === "dark") {
      return true;
    }

    return window.matchMedia("(prefers-color-scheme: dark)").matches;
  };

  const apply = (preference) => {
    const root = document.documentElement;
    const resolvedDark = resolveMode(preference);
    root.classList.toggle("dark", resolvedDark);
    root.setAttribute("data-theme-preference", preference);
    return { preference, isDark: resolvedDark };
  };

  const getStoredPreference = () => localStorage.getItem(storageKey) || "system";

  const bootstrap = () => apply(getStoredPreference());

  window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", () => {
    if (getStoredPreference() === "system") {
      apply("system");
    }
  });

  return {
    bootstrap,
    getThemeState: () => apply(getStoredPreference()),
    setPreference: (preference) => {
      localStorage.setItem(storageKey, preference);
      return apply(preference);
    }
  };
})();

window.libraryStore = {
  getFavorites: (storageKey) => {
    try {
      const raw = localStorage.getItem(storageKey);
      return raw ? JSON.parse(raw) : [];
    } catch {
      return [];
    }
  },
  setFavorites: (storageKey, values) => {
    localStorage.setItem(storageKey, JSON.stringify(values ?? []));
  }
};

window.collinPlayer = (() => {
  let currentAudio = null;
  let dotNetRef = null;
  let listeners = [];

  const cleanup = () => {
    for (const { eventName, handler } of listeners) {
      currentAudio?.removeEventListener(eventName, handler);
    }

    listeners = [];
  };

  const reportTimeline = () => {
    if (!currentAudio || !dotNetRef) {
      return;
    }

    dotNetRef.invokeMethodAsync(
      "HandleTimelineChanged",
      currentAudio.currentTime || 0,
      Number.isFinite(currentAudio.duration) ? currentAudio.duration : 0,
      currentAudio.paused === false,
      currentAudio.volume || 0
    );
  };

  const bind = (audioElement, ref) => {
    if (currentAudio !== audioElement) {
      cleanup();
    }

    currentAudio = audioElement;
    dotNetRef = ref;

    const eventNames = ["timeupdate", "loadedmetadata", "play", "pause", "volumechange", "ended"];
    for (const eventName of eventNames) {
      const handler = () => {
        if (eventName === "ended") {
          dotNetRef.invokeMethodAsync("HandleEnded");
          return;
        }

        reportTimeline();
      };

      audioElement.addEventListener(eventName, handler);
      listeners.push({ eventName, handler });
    }

    reportTimeline();
  };

  return {
    init: (audioElement, ref) => bind(audioElement, ref),
    setSource: async (audioElement, src, autoplay) => {
      if (!audioElement) {
        return;
      }

      if (audioElement.getAttribute("src") !== src) {
        audioElement.setAttribute("src", src);
        audioElement.load();
      }

      if (autoplay) {
        try {
          await audioElement.play();
        } catch {
        }
      }
    },
    play: async (audioElement) => {
      if (!audioElement) {
        return;
      }

      try {
        await audioElement.play();
      } catch {
      }
    },
    pause: (audioElement) => audioElement?.pause(),
    setVolume: (audioElement, volume) => {
      if (audioElement) {
        audioElement.volume = volume;
      }
    },
    seek: (audioElement, value) => {
      if (audioElement) {
        audioElement.currentTime = value;
      }
    }
  };
})();
