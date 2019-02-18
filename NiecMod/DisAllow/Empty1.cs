using System;
using System.Runtime.InteropServices;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;

namespace Sims3.SimIFacea
{
	// Token: 0x020001E8 RID: 488
	[ComVisible(false)]
	public struct ThumbnailKey
	{
		// Token: 0x06000D44 RID: 3396 RVA: 0x00014D9C File Offset: 0x00013D9C
		public ThumbnailKey(ResourceKey objectDescKey, ThumbnailSize size)
		{
			this.mDescKey = objectDescKey;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = 0;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = 0u;
			this.mAgeGender = 0u;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00014E10 File Offset: 0x00013E10
		public ThumbnailKey(ResourceKey objectDescKey, ThumbnailSize size, uint geoState, uint matState)
		{
			this.mDescKey = objectDescKey;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = 0;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = 0u;
			this.mAgeGender = 0u;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = matState;
			this.mGeometryState = geoState;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00014E80 File Offset: 0x00013E80
		public ThumbnailKey(ObjectGuid templateObjectId, int index, ThumbnailSize size)
		{
			this.mDescKey = default(ResourceKey);
			this.mTemplateObjectId = templateObjectId;
			this.mIndex = index;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = 0u;
			this.mAgeGender = 0u;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00014EF4 File Offset: 0x00013EF4
		public ThumbnailKey(ObjectGuid templateObjectId, int index, ThumbnailSize size, ThumbnailCamera camera)
		{
			this.mDescKey = default(ResourceKey);
			this.mTemplateObjectId = templateObjectId;
			this.mIndex = index;
			this.mSize = size;
			this.mCamera = camera;
			this.mBodyType = 0u;
			this.mAgeGender = 0u;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x00014F68 File Offset: 0x00013F68
		public ThumbnailKey(ResourceKey objectDescKey, uint bodyType, uint ageGender, ThumbnailSize size)
		{
			this.mDescKey = objectDescKey;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = -1;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = bodyType;
			this.mAgeGender = ageGender;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00014FDA File Offset: 0x00013FDA
		public ThumbnailKey(ResourceKey objectDescKey, uint bodyType, uint ageGender, bool liveUpdate, ThumbnailSize size)
		{
			this = new ThumbnailKey(objectDescKey, bodyType, ageGender, size);
			this.mLiveUpdate = liveUpdate;
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x00014FF0 File Offset: 0x00013FF0
		public ThumbnailKey(ResourceKey objectDescKey, int index, uint bodyType, uint ageGender, ThumbnailSize size)
		{
			this.mDescKey = objectDescKey;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = index;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = bodyType;
			this.mAgeGender = ageGender;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x00015063 File Offset: 0x00014063
		public ThumbnailKey(ResourceKey objectDescKey, int index, uint bodyType, uint ageGender, bool liveUpdate, ThumbnailSize size)
		{
			this = new ThumbnailKey(objectDescKey, index, bodyType, ageGender, size);
			this.mLiveUpdate = liveUpdate;
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x0001507C File Offset: 0x0001407C
		public ThumbnailKey(ulong lotId, uint level, bool bWallUp, bool bRoofUp, ThumbnailSize size)
		{
			this.mDescKey.InstanceId = lotId;
			this.mDescKey.TypeId = 1131408972u;
			if (bRoofUp)
			{
				this.mDescKey.GroupId = 0u;
			}
			else
			{
				this.mDescKey.GroupId = 2u * level + 1u;
				if (bWallUp)
				{
					this.mDescKey.GroupId = this.mDescKey.GroupId + 1u;
				}
			}
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = 0;
			this.mSize = size;
			this.mCamera = ThumbnailCamera.Default;
			this.mBodyType = 0u;
			this.mAgeGender = 0u;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0001513C File Offset: 0x0001413C
		public ThumbnailKey(SimOutfit outfit, int index, ThumbnailSize size, ThumbnailCamera camera)
		{
			this.mDescKey = outfit.Key;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = index;
			this.mSize = size;
			this.mCamera = camera;
			this.mBodyType = 0u;
			this.mAgeGender = (uint)outfit.AgeGenderSpecies;
			this.mLiveUpdate = false;
			this.mTechnique = ThumbnailTechnique.Default;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x000151B8 File Offset: 0x000141B8
		public ThumbnailKey(SimOutfit outfit, int index, ThumbnailSize size, ThumbnailCamera camera, uint ageGenderSpecies)
		{
			this = new ThumbnailKey(outfit, index, size, camera);
			this.mAgeGender = ageGenderSpecies;
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x000151D0 File Offset: 0x000141D0
		public ThumbnailKey(SimOutfit outfit, int index, ThumbnailSize size, ThumbnailCamera camera, ThumbnailTechnique technique)
		{
			this.mDescKey = outfit.Key;
			this.mTemplateObjectId = default(ObjectGuid);
			this.mIndex = index;
			this.mSize = size;
			this.mCamera = camera;
			this.mBodyType = 0u;
			this.mAgeGender = (uint)outfit.AgeGenderSpecies;
			this.mLiveUpdate = false;
			this.mTechnique = technique;
			this.mMaterialState = 0u;
			this.mGeometryState = 3787619543u;
			this.mDoNotCache = false;
			this.mAdditionalSizesToCache = ThumbnailSizeMask.None;
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x0001524D File Offset: 0x0001424D
		public ThumbnailKey(SimOutfit outfit, int index, bool liveUpdate, ThumbnailSize size, ThumbnailCamera camera)
		{
			this = new ThumbnailKey(outfit, index, size, camera);
			this.mLiveUpdate = liveUpdate;
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00015264 File Offset: 0x00014264
		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			ThumbnailKey thumbnailKey = (ThumbnailKey)obj;
			return this.mDescKey == thumbnailKey.mDescKey && this.mTemplateObjectId == thumbnailKey.mTemplateObjectId && this.mIndex == thumbnailKey.mIndex && this.mCamera == thumbnailKey.mCamera && this.mBodyType == thumbnailKey.mBodyType && this.mAgeGender == thumbnailKey.mAgeGender && this.mMaterialState == thumbnailKey.mMaterialState && this.mGeometryState == thumbnailKey.mGeometryState && this.mLiveUpdate == thumbnailKey.mLiveUpdate && this.mTechnique == thumbnailKey.mTechnique && this.mDoNotCache == thumbnailKey.mDoNotCache && this.mAdditionalSizesToCache == thumbnailKey.mAdditionalSizesToCache;
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x0001535E File Offset: 0x0001435E
		public override int GetHashCode()
		{
			if (this.mDescKey != ResourceKey.kInvalidResourceKey)
			{
				return this.mDescKey.GetHashCode();
			}
			return this.mTemplateObjectId.GetHashCode();
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x00015398 File Offset: 0x00014398
		public static bool operator !=(ThumbnailKey lhs, ThumbnailKey rhs)
		{
			return lhs.mDescKey != rhs.mDescKey || lhs.mTemplateObjectId != rhs.mTemplateObjectId || lhs.mIndex != rhs.mIndex || lhs.mCamera != rhs.mCamera || lhs.mBodyType != rhs.mBodyType || lhs.mAgeGender != rhs.mAgeGender || lhs.mMaterialState != rhs.mMaterialState || lhs.mGeometryState != rhs.mGeometryState || lhs.mLiveUpdate != rhs.mLiveUpdate || lhs.mTechnique != rhs.mTechnique || lhs.mDoNotCache != rhs.mDoNotCache || lhs.mAdditionalSizesToCache != rhs.mAdditionalSizesToCache;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x00015480 File Offset: 0x00014480
		public static bool operator ==(ThumbnailKey lhs, ThumbnailKey rhs)
		{
			return lhs.mDescKey == rhs.mDescKey && lhs.mTemplateObjectId == rhs.mTemplateObjectId && lhs.mIndex == rhs.mIndex && lhs.mCamera == rhs.mCamera && lhs.mBodyType == rhs.mBodyType && lhs.mAgeGender == rhs.mAgeGender && lhs.mMaterialState == rhs.mMaterialState && lhs.mGeometryState == rhs.mGeometryState && lhs.mLiveUpdate == rhs.mLiveUpdate && lhs.mTechnique == rhs.mTechnique && lhs.mDoNotCache == rhs.mDoNotCache && lhs.mAdditionalSizesToCache == rhs.mAdditionalSizesToCache;
		}

		// Token: 0x04000CDA RID: 3290
		private const uint kGeoStateThumbnail = 3787619543u;

		// Token: 0x04000CDB RID: 3291
		public const uint kSpecialKeyTypeForDownloadThumbnails = 3298902208u;

		// Token: 0x04000CDC RID: 3292
		public const uint kUrlImageThumbnails = 3692018064u;

		// Token: 0x04000CDD RID: 3293
		public ResourceKey mDescKey;

		// Token: 0x04000CDE RID: 3294
		public ObjectGuid mTemplateObjectId;

		// Token: 0x04000CDF RID: 3295
		public int mIndex;

		// Token: 0x04000CE0 RID: 3296
		public ThumbnailSize mSize;

		// Token: 0x04000CE1 RID: 3297
		public ThumbnailCamera mCamera;

		// Token: 0x04000CE2 RID: 3298
		public uint mBodyType;

		// Token: 0x04000CE3 RID: 3299
		public uint mAgeGender;

		// Token: 0x04000CE4 RID: 3300
		public uint mMaterialState;

		// Token: 0x04000CE5 RID: 3301
		public uint mGeometryState;

		// Token: 0x04000CE6 RID: 3302
		public bool mLiveUpdate;

		// Token: 0x04000CE7 RID: 3303
		public ThumbnailTechnique mTechnique;

		// Token: 0x04000CE8 RID: 3304
		public bool mDoNotCache;

		// Token: 0x04000CE9 RID: 3305
		public ThumbnailSizeMask mAdditionalSizesToCache;

		// Token: 0x04000CEA RID: 3306
		public static readonly ThumbnailKey kInvalidThumbnailKey = default(ThumbnailKey);
	}
}
