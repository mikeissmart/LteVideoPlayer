//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

export interface IConvertFileDto
{
	id?: number;
	output?: string;
	errored?: boolean;
	originalFile?: IFileDto;
	convertedFile?: IFileDto;
	audioStream?: number;
	createdDate?: Date;
	startedDate?: Date;
	endedDate?: Date;
}
export interface IConvertFileQueueDto
{
	index?: number;
	skip?: boolean;
	convertQueued?: boolean;
	convertName?: string;
	appendConvertName?: string;
	file?: IFileDto;
}
export interface IConvertManyFileDto
{
	converts?: IConvertFileDto[];
}
export interface ICreateConvertDto
{
	originalFile?: IFileDto;
	convertedFile?: IFileDto;
	audioStream?: number;
}
export interface ICreateManyConvertDto
{
	converts?: ICreateConvertDto[];
}
export interface IDirDto
{
	dirPath?: string;
	dirName?: string;
	dirPathName?: string;
}
export interface IDirsAndFilesDto
{
	dirs?: IDirDto[];
	files?: IFileDto[];
}
export interface IFileDto
{
	filePath?: string;
	fileName?: string;
	fileExists?: boolean;
	convertQueued?: boolean;
	filePathName?: string;
	fileNameWithoutExtension?: string;
	filePathNameWithoutExtension?: string;
}
export interface IFileHistoryDto
{
	id?: number;
	userProfileId?: number;
	percentWatched?: number;
	fileEntity?: IFileDto;
	startedDate?: Date;
}
export interface IMetaDataDto
{
	output?: string;
	error?: string;
}
export interface IRemoteDataDto
{
	profile?: string;
	channel?: number;
}
export interface IRemoteData_FullScreenDto extends IRemoteDataDto
{
}
export interface IRemoteData_MoveSeekDto extends IRemoteDataDto
{
	seekPosition?: number;
}
export interface IRemoteData_SetSeekDto extends IRemoteDataDto
{
	seekPercentPosition?: number;
}
export interface IRemoteData_SetVolumeDto extends IRemoteDataDto
{
	volume?: number;
}
export interface IRemoteData_VideoInfoDto extends IRemoteDataDto
{
	videoFile?: string;
	currentTimeSeconds?: number;
	maxTimeSeconds?: number;
	volume?: number;
	isPlaying?: boolean;
}
export interface IStringDto
{
	data?: string;
}
export interface IThumbnailErrorDto
{
	timesFailed?: number;
	error?: string;
	file?: IFileDto;
	lastError?: Date;
}
export interface IUserProfileDto
{
	id?: number;
	name?: string;
	createdDate?: Date;
}
